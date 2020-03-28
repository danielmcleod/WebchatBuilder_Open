using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ININ.IceLib.Connection;
using ININ.IceLib.Interactions;
using WebChatBuilderModels;

namespace WebChatBuilderService.Icelib.Interactions
{
    class WorkgroupInteractions
    {
        private Repository _repository;
        private InteractionsManager _interactionsManager;
        private Logging _logging;

        private ICollection<InteractionQueue> _queues;
        private readonly string[] _queueAttributes = new[]
        {
            InteractionAttributeName.InteractionId,
            "WebTools_WebChatId"
        };

        public static bool RefreshWatch = false;

        protected Repository Repository
        {
            get { return _repository ?? (_repository = new Repository()); }
        }
        
        public void Load(Session session)
        {
            _interactionsManager = InteractionsManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartOrRefreshWatchingInteractions();
        }

        public void Unload()
        {
            try
            {
                foreach (var interactionQueue in _queues)
                {
                    if (interactionQueue.IsWatching())
                    {
                        interactionQueue.StopWatching();
                    }
                }
                _queues = null;
            }
            catch (Exception)
            {
            }
            _interactionsManager = null;
        }

        public void RefreshWatches()
        {
            if (RefreshWatch)
            {
                RefreshWatch = false;
                StartOrRefreshWatchingInteractions();
            }
        }

        private void StartOrRefreshWatchingInteractions()
        {
            if (_queues == null)
            {
                _queues = new Collection<InteractionQueue>();

                try
                {
                    var workgroups = Repository.Workgroups.Where(w => w.Profiles.Any() && !w.MarkedForDeletion);
                    if (workgroups.Any() && _interactionsManager != null && _interactionsManager.Session != null && _interactionsManager.Session.ConnectionState == ConnectionState.Up)
                    {
                        foreach (var workgroup in workgroups)
                        {
                            try
                            {
                                var interactionQueue = new InteractionQueue(_interactionsManager, new QueueId(QueueType.Workgroup, workgroup.DisplayName));
                                interactionQueue.InteractionAdded += QueueOnInteractionAdded;
                                interactionQueue.StartWatchingAsync(_queueAttributes, Interactions_StartOrRefreshWatchingCompleted, null);
                                _queues.Add(interactionQueue);
                            }
                            catch (Exception e)
                            {
                                _logging.TraceException(e, "WorkgroupInteractions Error");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logging.TraceException(e, "WorkgroupInteractions Error");
                }

            }
            else
            {
                var workgroups = Repository.Workgroups.Where(w => w.Profiles.Any() && !w.MarkedForDeletion).ToList();
                if (workgroups.Any())
                {
                    foreach (var workgroup in workgroups)
                    {
                        var existing = _queues.FirstOrDefault(q => q.QueueId.QueueName == workgroup.DisplayName);
                        if (existing == null)
                        {
                            try
                            {
                                var interactionQueue = new InteractionQueue(_interactionsManager, new QueueId(QueueType.Workgroup, workgroup.DisplayName));
                                interactionQueue.InteractionAdded += QueueOnInteractionAdded;
                                interactionQueue.StartWatchingAsync(_queueAttributes, Interactions_StartOrRefreshWatchingCompleted, null);
                                _queues.Add(interactionQueue);
                            }
                            catch (Exception e)
                            {
                                _logging.TraceException(e, "WorkgroupInteractions Error");
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
                                    _queues.Remove(existing);
                                }
                                catch (Exception e)
                                {
                                    _logging.TraceException(e, "WorkgroupInteractions Error");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void QueueOnInteractionAdded(object sender, InteractionAttributesEventArgs e)
        {
            var repository = new Repository();
            var interaction = e.Interaction;
            if (interaction is ChatInteraction)
            {
                var chatId = interaction.GetWatchedStringAttribute("WebTools_WebChatId");
                var interactionId = interaction.GetWatchedStringAttribute(InteractionAttributeName.InteractionId);
                if (!String.IsNullOrEmpty(chatId) && !String.IsNullOrEmpty(interactionId))
                {
                    var chatIdLong = Convert.ToInt64(chatId);
                    var chat = repository.Chats.FirstOrDefault(c => c.ChatId == chatIdLong);
                    if (chat != null)
                    {
                        chat.InteractionId = interactionId;
                        repository.SaveChanges();
                    }
                }
            }
        }

        private void Interactions_StartOrRefreshWatchingCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

    }
}
