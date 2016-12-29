using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Build.WebApi;
using System.Collections;
using Microsoft.VisualStudio.Services.WebApi;
using MargieBot;

namespace SoftwareBot
{
    public class TfsBuildResponder : ISBResponder
    {
        List<string> taskNums = new List<string>();
        #region Constants
        private static Regex BUILD_MASK = new Regex(@"build status(?<param>\s.*)?$");
        private static Regex PROJECT_MASK = new Regex(@"project (?<project>\w+)");
        private static Regex COLLECTION_MASK = new Regex(@"collection (?<collection>\w+)");
        private static Regex PERSON_MASK = new Regex(@"for (?<person>all|me|.*)");
        private static Regex STARTTIME_MASK = new Regex(@"from (?<startdate>\d{4}-\d{2}-\d{2})");
        private static Regex ENDTIME_MASK = new Regex(@"to (?<enddate>\d{4}-\d{2}-\d{2})");
        private static string CLASSTOSTRING = "TFS Build responder";
        private static string DATE_PATTERN = "yyyy-MM-dd";
        private static string USAGE = "build status [from YYYY-MM-DD] [to YYYY-MM-DD] [collection collectionName] [project projectName] [for name]";
        private static string DESCRIPTION = @"Replies with a message with build status.
[from YYYY-MM-DD]        : beginning time of build finish time.    Default to today 00:00.
[to YYYY-MM-DD]            : end time of build finish time.          Default to now.
[collection collectionName]: collection name.                        Default to MainProjects.
[project projectName]      : project name.                           Default to IAFCore.
[for name]                 : the name of the person requested build. Default to me. Can be me, all, or a name.
[for name] must be the last parameter, since it will treat all words after 'for' as a name.";
        #endregion

        public bool CanRespond(ResponseContext context)
        {
            bool IsBotRespond = !context.BotHasResponded;
            bool IsBotMentioned = context.Message.MentionsBot;
            bool IsCommandIssued = BUILD_MASK.IsMatch(context.Message.Text);

            return IsBotMentioned && IsBotRespond && IsCommandIssued;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public BotMessage GetResponse(ResponseContext context)
        {
            BotMessage message = new BotMessage();
            Match match = BUILD_MASK.Match(context.Message.Text);
            string rc = "Match: {0}, param: {1}\nProject: {2}, Collection: {3}, Person: {4}";
            string parameters = match.Groups["param"].Success ? match.Groups["param"].Value : "";
            string project = PROJECT_MASK.Match(parameters).Success ? PROJECT_MASK.Match(parameters).Groups["project"].Value : "IAFCore";
            string collection = COLLECTION_MASK.Match(parameters).Success ? COLLECTION_MASK.Match(parameters).Groups["collection"].Value : "MainProjects";
            string person = PERSON_MASK.Match(parameters).Success ? PERSON_MASK.Match(parameters).Groups["person"].Value : "me" ;
            string startDate = STARTTIME_MASK.Match(parameters).Success ? STARTTIME_MASK.Match(parameters).Groups["startdate"].Value : null;
            string endDate = ENDTIME_MASK.Match(parameters).Success ? ENDTIME_MASK.Match(parameters).Groups["enddate"].Value : null;
            Console.Error.WriteLine(String.Format(rc, match.Success, parameters, project, collection, person));

            if (match.Success)
            {
                string fullName;

                switch (person)
                {
                    case "me":
                        fullName = Utility.SearchUserByID(context.Message.User.ID).profile.real_name;
                        person = fullName;
                        break;
                    case "all":
                        fullName = "";
                        break;
                    default:
                        fullName = person;
                        break;
                }


                message.Text = $"<@{context.Message.User.ID}> Project *{project}* in Collection *{collection}*. Build result(s) from *{person}*.";

                ArrayList list = getBuilds(collection, project, fullName, startDate, endDate);

                if (list.Count == 0)
                {
                    SlackAttachment sa = new SlackAttachment();
                    sa.Text = "Really empty here...";
                    message.Attachments.Add(sa);
                }
                else
                {
                    foreach (SlackAttachment a in list)
                    {
                        if (message.Attachments.Count < 19)
                        {
                            message.Attachments.Add(a);
                        }
                        else
                        {
                            SlackAttachment sa = new SlackAttachment();
                            sa.Text = "... and more";
                            message.Attachments.Add(sa);
                            break;
                        }

                    }
                }

                return message;
            }
            else
            {
                message.Text = "I cannot understand you...";
            }
            return message;
        }



        protected ArrayList getBuilds(string collection, string project, string username, string startDate, string endDate)
        {
            ArrayList list = new ArrayList();
            

            try
            {
                
                DateTime buildStartDate;
                DateTime buildEndDate;
                bool IsMinBuildFinishTimeExists = DateTime.TryParseExact(startDate, DATE_PATTERN, null, System.Globalization.DateTimeStyles.None, out buildStartDate);
                bool IsMaxBuildFinishTimeExists = DateTime.TryParseExact(endDate, DATE_PATTERN, null, System.Globalization.DateTimeStyles.None, out buildEndDate);
                VssConnection connection = new VssConnection(new Uri($"http://tfs.itracks.com:8080/tfs/{collection}"), new VssAadCredential());
                var projectClient = connection.GetClient<ProjectHttpClient>();
                var target = projectClient.GetProject(project).Result;
                var buildClient = connection.GetClient<BuildHttpClient>();

                // Build finish time process.
                // If start time not given, then set it to today 00:00;
                if (!IsMinBuildFinishTimeExists)
                {
                    buildStartDate = DateTime.Today;
                }

                // If end time not given, then set it to now.
                if (!IsMaxBuildFinishTimeExists)
                {
                    buildEndDate = DateTime.Now;
                }
                else
                {
                    // If end time is today, then set it to now.
                    // Else, set it to the end of that day.
                    if (buildEndDate.Date.Equals(DateTime.Today.Date))
                    {
                        buildEndDate = DateTime.Now;
                    }
                    else
                    {
                        buildEndDate = buildEndDate.AddHours(24);
                    }
                }

                var builds = buildClient.GetBuildsAsync(target.Id, minFinishTime: buildStartDate, maxFinishTime: buildEndDate).Result;
                if (builds.Count > 0)
                {
                    // For each build obtained, create a slack attachment.
                    // Only pick those builds which fullname contains given name.
                    // Even there are more than 20 builds, all builds will be returned as Slack attachment.
                    // 
                    foreach(var build in builds)
                    {
                        SlackAttachment buildAttachment = new SlackAttachment();
                        SlackAttachmentField buildAttachmentField = new SlackAttachmentField();

                        if (!build.RequestedFor.DisplayName.Contains(username))
                        {
                            continue;
                        }

                        ReferenceLink buildLink = (ReferenceLink) build.Links.Links["web"];
                        if (build.Status == BuildStatus.Completed)
                        {
                            buildAttachment.ColorHex = "good";
                            buildAttachment.TitleLink = buildLink.Href;
                            buildAttachment.Title = $"Branch: {build.SourceBranch}, ChangeSet {build.SourceVersion}";
                            buildAttachment.Text = $"Completed: {build.FinishTime.Value.ToLocalTime()}";
                        }
                        else if (build.Status == BuildStatus.InProgress)
                        {
                            buildAttachment.ColorHex = "warning";
                            buildAttachment.TitleLink = buildLink.Href;
                            buildAttachment.Title = $"Branch: {build.SourceBranch}, ChangeSet {build.SourceVersion}";
                            buildAttachment.Text = $"Started: {build.StartTime.Value.ToLocalTime()}";
                        }
                        else
                        {
                            buildAttachment.ColorHex = "danger";
                            buildAttachment.TitleLink = buildLink.Href;
                            buildAttachment.Title = $"Branch: {build.SourceBranch}, ChangeSet {build.SourceVersion}";
                            if (build.Status.HasValue)
                            {
                                buildAttachment.Text = $"Status: {build.Status.ToString()}";
                            }
                            else
                            {
                                buildAttachment.Text = "Status: unknown";
                            }
                        }
                        buildAttachmentField.Title = "Build for:";
                        buildAttachmentField.Value = build.RequestedFor.DisplayName;
                        buildAttachment.Fields.Add(buildAttachmentField);
                        list.Add(buildAttachment);
                    }
                }

                

                return list;
            }
            // When it hits an error, add the error in the list and return the list immediately. 
            catch (Exception e)
            {
                SlackAttachment attachment = new SlackAttachment();
                Exception ie;
                string IsInnerException;
                if (e.InnerException != null)
                {
                    ie = e.InnerException;
                    IsInnerException = "Warped";
                }
                else
                {
                    ie = e;
                    IsInnerException = "Not Warped";
                }

                SlackAttachmentField field = new SlackAttachmentField();
                field.Title = $"Exception Class:{ie.GetType().ToString()}";
                field.Value = $"HResult: {ie.HResult.ToString()}";
                field.IsShort = false;

                SlackAttachmentField ErrorType = new SlackAttachmentField();
                field.Title = $"Exception Source:";
                field.Value = $"{IsInnerException}";
                field.IsShort = false;

                attachment.Fallback = $"Exception: {ie.Message}";
                attachment.ColorHex = "danger";
                attachment.Title = "Error occurs!";
                attachment.Text = $"{ie.Message}";

                attachment.Fields.Add(ErrorType);
                attachment.Fields.Add(field);
                list.Add(attachment);
                return list;
            }
        }
        public string getUsage() => USAGE;
        public string getDescription() => DESCRIPTION;
        public override string ToString() => CLASSTOSTRING;
    }
}
