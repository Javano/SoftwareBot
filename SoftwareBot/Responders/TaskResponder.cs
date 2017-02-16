using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using MargieBot;

namespace SoftwareBot
{
    public class TaskResponder : ISBResponder
    {
        List<string> taskNums = new List<string>();
        #region CONSTANTS
        private static string DESCRIPTION = "Responds with brief messages of specified TFS task.";
        private static string USAGE = "@{TASK_NUM}";
        private static string CLASSTOSTRING = "TFS Task Linker";
        private static string HTML_MARKER = @"<[^>]*>";
        private static Regex TASK_MASK = new Regex(@"\@\d+");
        #endregion
        public bool CanRespond(ResponseContext context)
        {
            taskNums = new List<string>();
            string messageLwr = context.Message.Text.ToLower();
            foreach (Match match in TASK_MASK.Matches(messageLwr))
            {
                taskNums.Add(match.Value.Substring(1));
            }

            return (taskNums.Count > 0 && !context.BotHasResponded);
        }
        public bool CanReact(ResponseContext context)
        {
            return false;
        }
        public BotReaction GetReaction(ResponseContext context) { return new BotReaction(); }

        public BotMessage GetResponse(ResponseContext context)
        {

            VssConnection connection = new VssConnection(new Uri("http://tfs.itracks.com:8080/tfs/MainProjects"), new VssAadCredential());
            List<SlackAttachment> list = new List<SlackAttachment>();
            var projectClient = connection.GetClient<ProjectHttpClient>();
            var target = projectClient.GetProject("IAFCore").Result;
            var workItemClient = connection.GetClient<WorkItemTrackingHttpClient>();
            Task<List<WorkItem>> task;
            BotMessage message = new BotMessage();
            try
            {
                task = workItemClient.GetWorkItemsAsync(taskNums.Select(int.Parse).ToList(), expand: WorkItemExpand.All);
                var workItems = task.Result;
                if (workItems != null)
                {
                    foreach (var item in workItems)
                    {
                        SlackAttachment workitem = new SlackAttachment();
                        SlackAttachmentField workitemStatus = new SlackAttachmentField { IsShort = true };
                        SlackAttachmentField workitemType = new SlackAttachmentField { IsShort = true };
                        SlackAttachmentField workitemAssigned = new SlackAttachmentField { IsShort = true };
                        workitem.Title = String.Format("{0:0000}: {1}", item.Fields["System.Id"], item.Fields["System.Title"]);
                        workitem.TitleLink = ((ReferenceLink)item.Links.Links["html"]).Href;
                        workitem.Fallback = workitem.Title;

                        workitemType.Title = "Type";
                        if (item.Fields.ContainsKey("System.WorkItemType"))
                        {
                            workitemType.Value = item.Fields["System.WorkItemType"].ToString();
                        }
                        else
                        {
                            workitemType.Value = "Unknown";
                        }


                        workitemStatus.Title = "State";
                        if (item.Fields.ContainsKey("System.State"))
                        {
                            workitemStatus.Value = item.Fields["System.State"].ToString();
                        }
                        else
                        {
                            workitemStatus.Value = "Unknown";
                        }


                        workitemAssigned.Title = "Assigned To";
                        if (item.Fields.ContainsKey("System.AssignedTo"))
                        {
                            string personAssigned = Regex.Replace(item.Fields["System.AssignedTo"].ToString(), HTML_MARKER, "");
                            workitemAssigned.Value = personAssigned;
                        }
                        else
                        {
                            workitemAssigned.Value = "None";
                        }

                        switch (workitemType.Value.ToLower())
                        {
                            case "bug":
                                workitem.ColorHex = "danger";
                                break;
                            case "product backlog item":
                                workitem.ColorHex = "good";
                                break;
                            case "task":
                                workitem.ColorHex = "warning";
                                break;
                            case "feature":
                                workitem.ColorHex = "#773b93";
                                break;
                            case "test case":
                            case "impediment":
                                workitem.ColorHex = "#439FE0";
                                break;
                            default:
                                workitem.ColorHex = "#FFA500";
                                workitem.Fields.Add(workitemType);
                                break;
                        }
                        workitem.Fields.Add(workitemStatus);
                        workitem.Fields.Add(workitemAssigned);

                        message.Attachments.Add(workitem);
                    }

                }
                else
                {
                    SlackAttachment workitem = new SlackAttachment();
                    workitem.Text = "Pretty empty here...";
                    message.Attachments.Add(workitem);
                }
                
            }
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

                message.Attachments.Add(attachment);
            }

            dynamic rData = Newtonsoft.Json.JsonConvert.DeserializeObject(context.Message.RawData);
            if (rData["ts"] != null)
            {
                string ts = rData["ts"];
                message.Thread_TS = ts;
            }
            if (rData["thread_ts"] != null)
            {
                string thread_ts = rData["thread_ts"];
                message.Thread_TS = thread_ts;
            }
            return message;
        }

        public string getUsage() => USAGE;
        public string getDescription() => DESCRIPTION;
        public override string ToString() => CLASSTOSTRING;
    }
}
