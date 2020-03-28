using System;
using System.Collections.Generic;
using System.Linq;
using ININ.IceLib.Configuration;
using ININ.IceLib.Configuration.DataTypes;
using ININ.IceLib.Connection;
using WebChatBuilderModels;
using MediaType = ININ.IceLib.Configuration.DataTypes.MediaType;

namespace WebChatBuilderService.Icelib.Configurations
{
    public class AgentLicenseConfigurations
    {
        private ConfigurationManager _configurationManager;
        private Logging _logging;

        protected Repository Repository
        {
            get { return new Repository(); }
        }

        public void Load(Session session)
        {
            _configurationManager = ConfigurationManager.GetInstance(session);
            _logging = Logging.GetInstance();
        }

        public void Unload()
        {
            _configurationManager = null;
        }

        public void RefreshWatches()
        {
            UpdateLicensing();
        }

        private void UpdateLicensing()
        {
            try
            {
                using (var repository = Repository)
                {
                    if (_configurationManager != null && _configurationManager.Session != null && _configurationManager.Session.ConnectionState == ConnectionState.Up)
                    {
                        var userConfigurationList = new UserConfigurationList(_configurationManager);
                        var agents = repository.Agents.Where(a => a.ActiveInWorkgroups.Any());
                        if (agents.Any())
                        {
                            var querySettings = userConfigurationList.CreateQuerySettings();
                            querySettings.SetResultCountLimit(QueryResultLimit.Unlimited);

                            var properties = new List<UserConfiguration.Property>
                            {
                                UserConfiguration.Property.Id,
                                UserConfiguration.Property.License_LicenseActive,
                                UserConfiguration.Property.License_HasClientAccess,
                                UserConfiguration.Property.License_MediaLevel,
                                UserConfiguration.Property.License_MediaTypes
                            };

                            querySettings.SetPropertiesToRetrieve(properties);

                            //var userFilterDefinitions = agents.Select(a => a.DisplayName).ToList().Select(agentName => new BasicFilterDefinition<UserConfiguration, UserConfiguration.Property>(UserConfiguration.Property.DisplayName, agentName, FilterMatchType.Contains)).Cast<FilterDefinition<UserConfiguration, UserConfiguration.Property>>().ToList();

                            //var agentConfigIds = agents.Select(a => a.ConfigId).ToList();

                            //var userFilterDefinitions = new List<FilterDefinition<UserConfiguration, UserConfiguration.Property>>();
                            //foreach (var agentConfigId in agentConfigIds)
                            //{
                            //    userFilterDefinitions.Add(new BasicFilterDefinition<UserConfiguration, UserConfiguration.Property>(UserConfiguration.Property.Id, agentConfigId, FilterMatchType.Exact));
                            //}

                            //querySettings.SetFilterDefinition(new GroupFilterDefinition<UserConfiguration, UserConfiguration.Property>(userFilterDefinitions));

                            userConfigurationList.StartCaching(querySettings);
                            var users = userConfigurationList.GetConfigurationList();
                            userConfigurationList.StopCaching();
                            foreach (var userConfiguration in users)
                            {
                                var hasClientAccess = userConfiguration.License.HasClientAccess.Value && userConfiguration.License.LicenseActive.Value;
                                var licenseMediaLevel = userConfiguration.License.MediaLevel.Value;
                                var mediaLevel = licenseMediaLevel == MediaLevel.Media1 ? 1 : licenseMediaLevel == MediaLevel.Media2 ? 2 : licenseMediaLevel == MediaLevel.Media3 ? 3 : 0;
                                var mediaTypes = userConfiguration.License.MediaTypes.Value;
                                var isLicensedForChat = mediaTypes.Contains(MediaType.Chat);
                                var user = repository.Agents.FirstOrDefault(a => a.ConfigId == userConfiguration.ConfigurationId.Id);
                                if (user != null)
                                {
                                    if (user.HasActiveClientLicense != hasClientAccess || user.IsLicensedForChat != isLicensedForChat || user.MediaLevel != mediaLevel)
                                    {
                                        user.HasActiveClientLicense = hasClientAccess;
                                        user.MediaLevel = mediaLevel;
                                        user.IsLicensedForChat = isLicensedForChat;
                                        repository.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logging.TraceException(e, "AgentLicenseConfigurations Error");
            }
        }
    }
}
