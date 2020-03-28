using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ININ.IceLib.Connection;
using ININ.IceLib.People;
using WebChatBuilderModels;

namespace WebChatBuilderService.Icelib.People
{
    public class WorkgroupPeople
    {
        private Repository _repository;
        private PeopleManager _peopleManager;
        private Logging _logging;

        private ICollection<WorkgroupDetails> _workgroupDetails;
        private readonly string[] _watchedAttributes = { WorkgroupAttributes.ActiveMembers, WorkgroupAttributes.Members, WorkgroupAttributes.HasQueue, WorkgroupAttributes.IsActive, WorkgroupAttributes.QueueType };

        public static bool RefreshWatch = false;

        protected Repository Repository
        {
            get { return _repository ?? (_repository = new Repository()); }
        }

        public void Load(Session session)
        {
            _peopleManager = PeopleManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartOrRefreshWatchingWorkgroups();
        }

        public void Unload()
        {
            try
            {
                StopWatchingWorkgroups();
            }
            catch (Exception)
            {
            }
        }

        public void RefreshWatches()
        {
            if (RefreshWatch)
            {
                RefreshWatch = false;
                StartOrRefreshWatchingWorkgroups();
            }
        }

        private void StartOrRefreshWatchingWorkgroups()
        {
            _logging.LogNote("StartOrRefreshWatchingWorkgroups");
            if (_workgroupDetails == null)
            {
                _workgroupDetails = new Collection<WorkgroupDetails>();

                try
                {//Todo: We should only lookup workgroups that are configured for profiles and we should trigger an update from the wcb web app to the wcb service
                 //when profile workgroups change or use assignable workgroups and trigger when marked as assignable or on added and assignable.
                    var workgroups = Repository.Workgroups.Where(i => i.Profiles.Any() && !i.MarkedForDeletion).ToList();//.Where(w => w.Profiles.Any() && !w.MarkedForDeletion);
                    if (workgroups.Any() && _peopleManager != null && _peopleManager.Session != null && _peopleManager.Session.ConnectionState == ConnectionState.Up)
                    {
                        foreach (var workgroup in workgroups.Distinct().ToList())
                        {
                            try
                            {
                                var workgroupDetails = new WorkgroupDetails(_peopleManager, workgroup.DisplayName);
                                workgroupDetails.WatchedAttributesChanged += WatchedAttributesChanged;
                                workgroupDetails.StartWatchingAsync(_watchedAttributes, WorkgroupDetails_StartWatchingCompleted,
                                    null);
                                _workgroupDetails.Add(workgroupDetails);
                            }
                            catch (Exception e)
                            {
                                _logging.LogException(e);
                                _logging.TraceException(e, "WorkgroupPeople Error");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logging.LogException(e);
                    _logging.TraceException(e, "WorkgroupPeople Error");
                }
            }
            else
            {
                try
                {
                    var workgroups = Repository.Workgroups.Where(w => w.Profiles.Any());
                    if (workgroups.Any())
                    {
                        foreach (var workgroup in workgroups)
                        {
                            var existing = _workgroupDetails.FirstOrDefault(w => w.Name == workgroup.DisplayName);
                            if (existing == null && _peopleManager != null && _peopleManager.Session != null && _peopleManager.Session.ConnectionState == ConnectionState.Up && !workgroup.MarkedForDeletion)
                            {
                                try
                                {
                                    var workgroupDetails = new WorkgroupDetails(_peopleManager, workgroup.DisplayName);
                                    workgroupDetails.WatchedAttributesChanged += WatchedAttributesChanged;
                                    workgroupDetails.StartWatchingAsync(_watchedAttributes,
                                        WorkgroupDetails_StartWatchingCompleted, null);
                                    _workgroupDetails.Add(workgroupDetails);
                                }
                                catch (Exception e)
                                {
                                    _logging.LogException(e);
                                    _logging.TraceException(e, "WorkgroupPeople Error");
                                }
                            }
                            else
                            {
                                if (workgroup.MarkedForDeletion)
                                {
                                    try
                                    {
                                        if (existing.IsWatching())
                                        {
                                            existing.StopWatching();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        _logging.LogException(e);
                                        _logging.TraceException(e, "WorkgroupPeople Error");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (existing.IsWatching())
                                        {
                                            existing.StopWatching();
                                            existing.StartWatchingAsync(_watchedAttributes, WorkgroupDetails_StartWatchingCompleted, null);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        _logging.LogException(e);
                                        _logging.TraceException(e, "WorkgroupPeople Error");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logging.LogException(e);
                    _logging.TraceException(e, "WorkgroupPeople Error");
                }
            }
        }

        private void WatchedAttributesChanged(object sender, WatchedAttributesEventArgs e)
        {
            _logging.LogNote("WatchedAttributesChanged");
            try
            {
                var repository = new Repository();
                var workgroupDetails = sender as WorkgroupDetails;
                if (workgroupDetails != null)
                {
                    _logging.LogNote("WorkgroupDetails: " + workgroupDetails.Name);
                    var isActive = workgroupDetails.IsActive;
                    var isAcd = workgroupDetails.QueueType.ToLower() == "acd" || workgroupDetails.QueueType.ToLower() == "custom";
                    var hasQueue = workgroupDetails.HasQueue;
                    var workgroup = repository.Workgroups.FirstOrDefault(w => w.DisplayName == workgroupDetails.Name);
                    if (workgroup != null)
                    {
                        _logging.LogNote("Workgroup: " + workgroup.DisplayName);
                        workgroup.IsActive = isActive;
                        workgroup.HasQueue = hasQueue;
                        workgroup.IsAcd = isAcd;
                        workgroup.ActiveMembers.Clear();
                        _logging.LogNote("Workgroups Active Members");
                        foreach (var activeMember in workgroupDetails.ActiveMembers.ToList())
                        {
                            _logging.LogNote(activeMember);
                            var agent = repository.Agents.FirstOrDefault(a => a.ConfigId.ToLower() == activeMember.ToLower());
                            if (agent != null)
                            {
                                workgroup.ActiveMembers.Add(agent);
                            }
                        }
                        repository.SaveChanges();
                    }
                    else
                    {
                        _logging.LogNote("workgroup is null");
                    }
                }
                else
                {
                    _logging.LogNote("WorkgroupDetails is null");
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupPeople Error");
            }
            _logging.LogNote("WatchedAttributesChanged Complete");
        }

        private void WorkgroupDetails_StopWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }

        private void WorkgroupDetails_StartWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _logging.LogNote("WorkgroupDetails_StartWatchingCompleted");
            try
            {
                var repository = new Repository();

                var workgroupDetails = sender as WorkgroupDetails;
                if (workgroupDetails != null)
                {
                    var isActive = workgroupDetails.IsActive;
                    var isAcd = workgroupDetails.QueueType.ToLower() == "acd" || workgroupDetails.QueueType.ToLower() == "custom";
                    var hasQueue = workgroupDetails.HasQueue;
                    var workgroup = repository.Workgroups.FirstOrDefault(w => w.DisplayName == workgroupDetails.Name);
                    if (workgroup != null)
                    {
                        _logging.LogNote("Workgroup: " + workgroup.DisplayName);
                        workgroup.IsActive = isActive;
                        workgroup.HasQueue = hasQueue;
                        workgroup.IsAcd = isAcd;
                        workgroup.ActiveMembers.Clear();
                        _logging.LogNote("Workgroups Active Members");
                        foreach (var activeMember in workgroupDetails.ActiveMembers.ToList())
                        {
                            _logging.LogNote(activeMember);
                            var agent = repository.Agents.FirstOrDefault(a => a.ConfigId.ToLower() == activeMember.ToLower());
                            if (agent != null)
                            {
                                workgroup.ActiveMembers.Add(agent);
                            }
                        }
                        repository.SaveChanges();
                    }
                }

            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupPeople Error");
            }
        }

        private void StopWatchingWorkgroups()
        {
            if (_workgroupDetails == null)
            {
                return;
            }
            foreach (var workgroupDetails in _workgroupDetails)
            {
                if (workgroupDetails.IsWatching())
                {
                    workgroupDetails.StopWatchingAsync(WorkgroupDetails_StopWatchingCompleted, null);
                }
                if (workgroupDetails == _workgroupDetails.Last())
                {
                    _peopleManager = null;
                }
            }

        }



    }
}
