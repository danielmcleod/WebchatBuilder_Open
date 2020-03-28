using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ININ.IceLib.Configuration;
using ININ.IceLib.Connection;
using WebChatBuilderModels;
using WebChatBuilderModels.Models;

namespace WebChatBuilderService.Icelib.Configurations
{
    public class SkillConfigurations
    {
        private ConfigurationManager _configurationManager;
        private Logging _logging;

        private SkillConfigurationList _skillConfigurationList;

        private ICollection<SkillConfiguration> _skills = new Collection<SkillConfiguration>();

        protected Repository Repository
        {
            get { return new Repository(); }
        }

        public void Load(Session session)
        {
            _configurationManager = ConfigurationManager.GetInstance(session);
            _logging = Logging.GetInstance();
            StartWatchingSkills();
        }

        public void Unload()
        {
            try
            {
                if (_skillConfigurationList.IsWatching)
                {
                    _skillConfigurationList.StopWatching();
                }
            }
            catch (Exception)
            {
            }
            _skillConfigurationList = null;
            _configurationManager = null;
        }

        public void RefreshWatches()
        {
            if (_skillConfigurationList.IsWatching)
            {
                _skillConfigurationList.RefreshWatchAsync(Skills_StartOrRefreshWatchingCompleted, null);
            }
        }

        private void StartWatchingSkills()
        {
            try
            {
                if (_configurationManager != null && _configurationManager.Session != null && _configurationManager.Session.ConnectionState == ConnectionState.Up)
                {
                    _skillConfigurationList = new SkillConfigurationList(_configurationManager);
                    var querySettings = _skillConfigurationList.CreateQuerySettings();

                    querySettings.SetResultCountLimit(QueryResultLimit.Unlimited);

                    //var properties = new List<SkillConfiguration.Property>
                    //{
                    //    SkillConfiguration.Property.Id,
                    //    SkillConfiguration.Property.DisplayName,
                    //    SkillConfiguration.Property.UserAssignments,
                    //    SkillConfiguration.Property.WorkgroupAssignments
                    //};

                    //querySettings.SetPropertiesToRetrieve(properties);

                    using (var repository = Repository)
                    {
                        if (!repository.Skills.Any())
                        {
                            _skillConfigurationList.StartCaching(querySettings);
                            var skills = _skillConfigurationList.GetConfigurationList();
                            _skillConfigurationList.StopCaching();
                            if (skills.Any())
                            {
                                foreach (var skillConfiguration in skills)
                                {
                                    CreateSkill(skillConfiguration.ConfigurationId.Id, skillConfiguration.ConfigurationId.DisplayName);
                                }
                                _skills = skills;
                            }
                        }
                    }

                    _skillConfigurationList.ConfigurationObjectsRemoved += Skills_ConfigurationObjectsRemoved;

                    _skillConfigurationList.StartWatchingAsync(querySettings, Skills_StartOrRefreshWatchingCompleted, null);
                }
            }
            catch (Exception e)
            {
                _logging.TraceException(e, "SkillConfigurations Error");
            }
        }

        private void Skills_StartOrRefreshWatchingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var skillConfigurationList = sender as SkillConfigurationList;
            ProcessSkillConfigurations(skillConfigurationList);
        }

        private static readonly object ProcessingLock = new object();

        private void ProcessSkillConfigurations(SkillConfigurationList skillConfigurationList)
        {
            lock (ProcessingLock)
            try
            {
                if (skillConfigurationList != null)
                {
                    var skills = skillConfigurationList.GetConfigurationList();
                    if (!_skills.Any() && skills.Any())
                    {
                        foreach (var skillConfiguration in skills)
                        {
                            CreateSkill(skillConfiguration.ConfigurationId.Id, skillConfiguration.ConfigurationId.DisplayName);
                        }
                        _skills = skills;
                        CheckForDeleted();
                    }
                    else if (SkillsChanged(skills))
                    {
                        foreach (var skillConfiguration in skills)
                        {
                            CreateSkill(skillConfiguration.ConfigurationId.Id, skillConfiguration.ConfigurationId.DisplayName);
                        }
                        _skills = skills;
                        CheckForDeleted();
                    }
                }
            }
            catch (Exception exception)
            {
                _logging.TraceException(exception, "SkillConfigurations Error");
            }
        }

        private void CreateSkill(string configId, string name)
        {
            try
            {
                using (var repository = Repository)
                {
                    var exist = repository.Skills.Any(s => s.ConfigId == configId);
                    if (!exist)
                    {
                        var skill = new Skill
                        {
                            ConfigId = configId,
                            DisplayName = name,
                            IsAssignable = false
                        };
                        repository.Skills.Add(skill);
                        repository.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                _logging.TraceException(e, "SkillConfigurations Error");
            }
        }

        private bool SkillsChanged(ICollection<SkillConfiguration> skills)
        {
            if (skills.Count != _skills.Count)
            {
                return true;
            }

            var skillIds = skills.Select(s => s.ConfigurationId.Id);
            var trackedSkillIds = _skills.Select(s => s.ConfigurationId.Id).ToList();
            return skillIds.Any(skillId => !trackedSkillIds.Contains(skillId));
        }

        private void Skills_ConfigurationObjectsRemoved(object sender, ConfigurationWatchEventArgs<SkillConfiguration> e)
        {
            try
            {
                var removeSkills = e.ObjectsAffected;
                using (var repository = Repository)
                {
                    var skills = repository.Skills;
                    foreach (var skill in removeSkills.Select(skillConfiguration => skills.FirstOrDefault(s => s.ConfigId == skillConfiguration.ConfigurationId.Id)).Where(skill => skill != null))
                    {
                        RemoveSkill(skill, repository);
                    }
                    repository.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                _logging.TraceException(exception, "SkillConfigurations Error");
            }
        }

        private void CheckForDeleted()
        {
            using (var repository = Repository)
            {
                var skillIds = repository.Skills.Where(i => !i.MarkedForDeletion).Select(s => s.ConfigId);
                var trackedSkillIds = _skills.Select(s => s.ConfigurationId.Id).ToList();
                var removed = skillIds.Where(skillId => !trackedSkillIds.Contains(skillId)).ToList();
                foreach (var removedSkill in removed)
                {
                    var skill = repository.Skills.FirstOrDefault(s => s.ConfigId == removedSkill);
                    if (skill != null)
                    {
                        RemoveSkill(skill, repository);
                    }
                }
            }
        }

        private void RemoveSkill(Skill skill, Repository repository)
        {
            try
            {
                if (skill.AssignedAgents != null)
                {
                    foreach (var agent in skill.AssignedAgents.ToList())
                    {
                        skill.AssignedAgents.Remove(agent);
                    }
                }
                if (skill.IsUsed)
                {
                    skill.MarkedForDeletion = true;
                }
                else
                {
                    repository.Skills.Remove(skill);
                }
            }
            catch (Exception exception)
            {
                _logging.TraceException(exception, "SkillConfigurations Error");
            }
        }

    }
}
