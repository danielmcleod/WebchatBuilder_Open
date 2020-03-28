using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.DataTypes;
using ININ.IceLib.Connection;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;
using WebChatBuilderService.Icelib.Interactions;
using WebChatBuilderService.Icelib.People;
using MediaType = ININ.IceLib.Configuration.DataTypes.MediaType;

namespace WebChatBuilderService.Icelib.Configurations
{
    public class WorkgroupConfigurations
    {
        private ConfigurationManager _configurationManager;
        private Logging _logging;

        private WorkgroupConfigurationList _workgroupConfigurationList;

        private ICollection<WorkgroupConfiguration> _workgroups = new Collection<WorkgroupConfiguration>();

        private static bool _firstRunAfterStart = true;

        private static bool _isStarting;
        private static bool _isUnloading;

        protected Repository Repository
        {
            get { return new Repository(); }
        }

        public void Load(Session session)
        {
            _isUnloading = false;
            _configurationManager = ConfigurationManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartWatchingWorkgroups();
        }

        public void Unload()
        {
            _isUnloading = true;
            try
            {
                if (_workgroupConfigurationList.IsWatching)
                {
                    _workgroupConfigurationList.StopWatching();
                }
            }
            catch (Exception)
            {
            }
            _workgroupConfigurationList = null;
            _configurationManager = null;
        }

        public void RefreshWatches()
        {
            if (_workgroupConfigurationList.IsWatching)
            {
                _workgroupConfigurationList.RefreshWatchAsync(Workgroups_StartOrRefreshWatchingCompleted, null);
            }
            else
            {
                if (!_isStarting && !_isUnloading)
                {
                    StartWatchingWorkgroups();
                }
            }
        }

        private void StartWatchingWorkgroups()
        {
            _isStarting = true;
            //_logging.LogNote("StartWatchingWorkgroups");
            try
            {
                if (_configurationManager != null && _configurationManager.Session != null && _configurationManager.Session.ConnectionState == ConnectionState.Up)
                {
                    _workgroupConfigurationList = new WorkgroupConfigurationList(_configurationManager);
                    var querySettings = _workgroupConfigurationList.CreateQuerySettings();
                    querySettings.SetResultCountLimit(QueryResultLimit.Unlimited);

                    var properties = new List<WorkgroupConfiguration.Property>
                    {
                        WorkgroupConfiguration.Property.Id,
                        WorkgroupConfiguration.Property.DisplayName,
                        WorkgroupConfiguration.Property.IsActive,
                        WorkgroupConfiguration.Property.HasQueue,
                        WorkgroupConfiguration.Property.QueueType,
                        WorkgroupConfiguration.Property.Utilizations
                    };

                    querySettings.SetPropertiesToRetrieve(properties);
                    using (var repository = Repository)
                    {
                        if (!repository.Workgroups.Any())
                        {
                            _workgroupConfigurationList.StartCaching(querySettings);
                            var workgroups = _workgroupConfigurationList.GetConfigurationList();
                            _workgroupConfigurationList.StopCaching();
                            if (workgroups.Any())
                            {
                                foreach (var workgroupConfiguration in workgroups)
                                {
                                    CreateWorkgroup(workgroupConfiguration);
                                }
                                _workgroups = workgroups;
                            }
                        }
                    }

                    _workgroupConfigurationList.ConfigurationObjectsRemoved += Workgroups_ConfigurationObjectsRemoved;

                    _workgroupConfigurationList.ConfigurationObjectsChanged += Workgroups_ConfigurationObjectsChanged;

                    _workgroupConfigurationList.StartWatchingAsync(querySettings, Workgroups_StartOrRefreshWatchingCompleted, null);
                }
            }
            catch (Exception e)
            {
                _logging.LogException(e);
                _logging.TraceException(e, "WorkgroupConfigurations Error");
                _isStarting = false;
            }
        }

        private void Workgroups_ConfigurationObjectsChanged(object sender, ConfigurationObjectChangesEventArgs<WorkgroupConfiguration, WorkgroupConfiguration.Property> e)
        {
            try
            {
                //_logging.LogNote("Workgroups_ConfigurationObjectsChanged");
                foreach (var changedObject in e.ObjectsChanged)
                {
                    var updated = false;
                    var workgroupConfiguration = changedObject.Key;

                    using (var repository = Repository)
                    {
                        var wg = repository.Workgroups.FirstOrDefault(w => w.ConfigId == workgroupConfiguration.ConfigurationId.Id);
                        if (wg != null)
                        {
                            //_logging.LogNote("Workgroup: " + wg.DisplayName);
                            if (changedObject.Value.Contains(WorkgroupConfiguration.Property.Utilizations))
                            {
                                //_logging.LogNote("Utilizations Changed");

                                var utilList = wg.Utilizations.ToList();
                                wg.Utilizations.Clear();
                                foreach (var utilization in utilList)
                                {
                                    repository.Utilizations.Remove(utilization);
                                }

                                var utilizations = workgroupConfiguration.Utilizations;
                                var utils = utilizations.Value;
                                var u = utils.Select(utilizationSettings => new Utilization
                                {
                                    MediaType = GetMediaType(utilizationSettings.MediaType),
                                    MaxAssignable = utilizationSettings.MaxAssignable,
                                    UtilizationPercent = utilizationSettings.Utilization
                                }).ToList();
                                repository.Utilizations.AddRange(u);
                                wg.Utilizations = u;
                                updated = true;
                                var utilizationString = String.Join(",", u.Select(i => i.MediaType));
                                //_logging.LogNote(String.Format("Utilizations: {0}", utilizationString));
                            }
                            if (updated)
                            {
                                repository.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupConfigurations Error");
            }
        }


        private void Workgroups_StartOrRefreshWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _isStarting = false;
            var workgroupConfigurationList = sender as WorkgroupConfigurationList;
            ProcessWorkgroupConfigurations(workgroupConfigurationList);
        }

        private static readonly object ProcessingLock = new object();

        private void ProcessWorkgroupConfigurations(WorkgroupConfigurationList workgroupConfigurationList)
        {
            lock (ProcessingLock)
            //_logging.LogNote("ProcessWorkgroupConfigurations");
            try
            {
                if (workgroupConfigurationList != null)
                {
                    var workgroups = workgroupConfigurationList.GetConfigurationList();
                    if (!_workgroups.Any() && workgroups.Any())
                    {
                        foreach (var workgroupConfiguration in workgroups)
                        {
                            CreateWorkgroup(workgroupConfiguration);
                        }
                        _workgroups = workgroups;
                        CheckForDeleted();
                    }
                    else if (WorkgroupsChanged(workgroups))
                    {
                        
                        foreach (var workgroupConfiguration in workgroups)
                        {
                            CreateWorkgroup(workgroupConfiguration, true);
                        }
                        _workgroups = workgroups;
                        CheckForDeleted();
                        WorkgroupPeople.RefreshWatch = true;
                        WorkgroupInteractions.RefreshWatch = true;
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupConfigurations Error");
            }
            _firstRunAfterStart = false;
        }

        private void CreateWorkgroup(WorkgroupConfiguration workgroupConfiguration, bool hasChanges = false)
        {
            //_logging.LogNote("CreateWorkgroup");
            try
            {
                using (var repository = Repository)
                {
                    var exist = repository.Workgroups.Any(w => w.ConfigId == workgroupConfiguration.ConfigurationId.Id);
                    if (!exist)
                    {
                        //_logging.LogNote("No Workgroups Exists");
                        var isActive = workgroupConfiguration.IsActive.Value;
                        var isAcd = workgroupConfiguration.QueueType.Value == WorkgroupQueueType.Acd ||
                                    workgroupConfiguration.QueueType.Value == WorkgroupQueueType.Custom;
                        var hasQueue = workgroupConfiguration.HasQueue.Value;
                        var utilizations = workgroupConfiguration.Utilizations;
                        var val = String.Join(",", utilizations.Value.Select(i => "MediaType:" + i.MediaType + " Percent:" + i.Utilization + " Max:" + i.MaxAssignable));
                        //_logging.LogNote(String.Format("Workgroup: {0} {1}", workgroupConfiguration.ConfigurationId.DisplayName, val));                        
                        var utils = utilizations.Value;
                        var u = utils.Select(utilizationSettings => new Utilization
                        {
                            MediaType = GetMediaType(utilizationSettings.MediaType),
                            MaxAssignable = utilizationSettings.MaxAssignable,
                            UtilizationPercent = utilizationSettings.Utilization
                        }).ToList();

                        var workgroup = new Workgroup()
                        {
                            ConfigId = workgroupConfiguration.ConfigurationId.Id,
                            DisplayName = workgroupConfiguration.ConfigurationId.DisplayName,
                            IsAcd = isAcd,
                            IsActive = isActive,
                            HasQueue = hasQueue,
                            MarkedForDeletion = false,
                            IsAssignable = false,
                            Utilizations = u
                        };
                        repository.Workgroups.Add(workgroup);
                        repository.SaveChanges();
                    }
                    else
                    {
                        if (_firstRunAfterStart || hasChanges)
                        {
                            //_logging.LogNote("First Run After Start");
                            var workgroup =
                                repository.Workgroups.FirstOrDefault(
                                    w => w.ConfigId == workgroupConfiguration.ConfigurationId.Id);
                            var utilizations = workgroupConfiguration.Utilizations;
                            var val = String.Join(",", utilizations.Value.Select(i => "MediaType:" + i.MediaType + " Percent:" + i.Utilization + " Max:" + i.MaxAssignable));
                            //_logging.LogNote(String.Format("Workgroup: {0} {1}", workgroupConfiguration.ConfigurationId.DisplayName, val));
                            var utils = utilizations.Value;
                            var u = utils.Select(utilizationSettings => new Utilization
                            {
                                MediaType = GetMediaType(utilizationSettings.MediaType),
                                MaxAssignable = utilizationSettings.MaxAssignable,
                                UtilizationPercent = utilizationSettings.Utilization
                            }).ToList();

                            if (workgroup != null)
                            {
                                var isActive = workgroupConfiguration.IsActive.Value;
                                var isAcd = workgroupConfiguration.QueueType.Value == WorkgroupQueueType.Acd ||
                                            workgroupConfiguration.QueueType.Value == WorkgroupQueueType.Custom;
                                var hasQueue = workgroupConfiguration.HasQueue.Value;

                                workgroup.ConfigId = workgroupConfiguration.ConfigurationId.Id;
                                workgroup.DisplayName = workgroupConfiguration.ConfigurationId.DisplayName;
                                workgroup.IsAcd = isAcd;
                                workgroup.IsActive = isActive;
                                workgroup.HasQueue = hasQueue;
                                workgroup.MarkedForDeletion = false;
                                var utilList = workgroup.Utilizations.ToList();
                                workgroup.Utilizations.Clear();
                                foreach (var utilization in utilList)
                                {
                                    repository.Utilizations.Remove(utilization);
                                }
                                workgroup.Utilizations = u;
                                //workgroup.IsAssignable = true;
                                repository.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logging.LogException(e);
                _logging.TraceException(e, "WorkgroupConfigurations Error");                
            }
        }

        private bool WorkgroupsChanged(ICollection<WorkgroupConfiguration> workgroups)
        {
            if (workgroups.Count != _workgroups.Count)
            {
                return true;
            }

            var workgroupIds = workgroups.Select(s => s.ConfigurationId.Id);
            var trackedWorkgroupIds = _workgroups.Select(w => w.ConfigurationId.Id).ToList();
            return workgroupIds.Any(workgroupId => !trackedWorkgroupIds.Contains(workgroupId));
        }

        //Check on this for Workgroup Dissapearing issue
        private void CheckForDeleted()
        {
            _logging.LogNote("Checking For Deleted Workgroups..");
            using (var repository = Repository)
            {
                var workgroupIds = repository.Workgroups.Where(i => !i.MarkedForDeletion).Select(w => w.ConfigId);
                var trackedWorkgroupIds = _workgroups.Select(w => w.ConfigurationId.Id).ToList();
                var removed = workgroupIds.Where(workgroupId => !trackedWorkgroupIds.Contains(workgroupId)).ToList();
                foreach (var removedWorkgroup in removed)
                {
                    var workgroup = repository.Workgroups.FirstOrDefault(w => w.ConfigId == removedWorkgroup);
                    if (workgroup != null)
                    {
                        _logging.LogNote("Removing: " + workgroup.DisplayName);
                        RemoveWorkgroup(workgroup, repository);
                    }
                }
                repository.SaveChanges();
            }
        }

        //Check on this for Workgroup Dissapearing issue
        private void Workgroups_ConfigurationObjectsRemoved(object sender, ConfigurationWatchEventArgs<WorkgroupConfiguration> e)
        {
            _logging.LogNote("Workgroup Configuration Objects Removed..");
            try
            {
                var removeWorkgroups = e.ObjectsAffected;
                using (var repository = Repository)
                {
                    var workgroups = repository.Workgroups;
                    foreach (var workgroup in removeWorkgroups.Select(workgroupConfiguration => workgroups.FirstOrDefault(w => w.ConfigId == workgroupConfiguration.ConfigurationId.Id)).Where(workgroup => workgroup != null))
                    {
                        _logging.LogNote("Removing: " + workgroup.DisplayName);
                        RemoveWorkgroup(workgroup,repository);
                    }
                    repository.SaveChanges();
                    WorkgroupPeople.RefreshWatch = true;
                    WorkgroupInteractions.RefreshWatch = true;
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupConfigurations Error");                
            }
        }

        private void RemoveWorkgroup(Workgroup workgroup, Repository repository)
        {
            try
            {
                if (workgroup.IsUsed)
                {
                    workgroup.MarkedForDeletion = true;
                }
                else
                {
                    if (workgroup.ActiveMembers != null)
                    {
                        foreach (var agent in workgroup.ActiveMembers.ToList())
                        {
                            workgroup.ActiveMembers.Remove(agent);
                        }
                    }
                    if (workgroup.Utilizations != null && workgroup.Utilizations.Any())
                    {
                        foreach (var workgroupUtilization in workgroup.Utilizations.ToList())
                        {
                            var utilization = repository.Utilizations.Find(workgroupUtilization.UtilizationId);
                            if (utilization != null)
                            {
                                workgroup.Utilizations.Remove(utilization);
                                repository.Utilizations.Remove(utilization);
                            }
                        }
                    }
                    repository.Workgroups.Remove(workgroup);
                }
            }
            catch (Exception exception)
            {
                _logging.LogException(exception);
                _logging.TraceException(exception, "WorkgroupConfigurations Error"); 
            }
        }

        private WebChatBuilderModels.Models.MediaType GetMediaType(MediaType mediaType)
        {
            if (mediaType == MediaType.Call)
            {
                return WebChatBuilderModels.Models.MediaType.Call;
            }
            if (mediaType == MediaType.Email)
            {
                return WebChatBuilderModels.Models.MediaType.Email;
            }
            if (mediaType == MediaType.Chat)
            {
                return WebChatBuilderModels.Models.MediaType.Chat;
            }
            if (mediaType == MediaType.Callback)
            {
                return WebChatBuilderModels.Models.MediaType.Callback;
            }
            if (mediaType == MediaType.Generic)
            {
                return WebChatBuilderModels.Models.MediaType.Generic;
            }
            if (mediaType == MediaType.WorkItem)
            {
                return WebChatBuilderModels.Models.MediaType.WorkItem;
            }
            return WebChatBuilderModels.Models.MediaType.Invalid;
        }

    }
}
