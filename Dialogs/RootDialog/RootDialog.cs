using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using System.IO;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.AI.QnA.Recognizers;
using AdaptiveExpressions;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Bot.Builder;
using CV_Chatbot.Constants;

namespace CV_Chatbot.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly IConfiguration _configuration;

        public RootDialog(IConfiguration configuration) : base(nameof(RootDialog))
        {
            _configuration = configuration;
            Recognizer = CreateCrossTrainedRecognizer(configuration);
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            // These steps are executed when this Adaptive Dialog begins
            Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath));
            Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },

                    new OnIntent(LuisConstant.STUDIES)
                    {
                        Condition = $"#{LuisConstant.STUDIES}.score >= 0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Studies()}")
                        }
                    },

                    new OnIntent(LuisConstant.WELCOME)
                    {
                        Condition = $"#{LuisConstant.WELCOME}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${WelcomeRetry()}"),
                            new SendActivity("${WelcomeActions()}")
                        },
                    },

                    new OnIntent(LuisConstant.EXPERIENCE)
                    {
                        Condition = $"#{LuisConstant.EXPERIENCE}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Experience()}")
                        },
                    },

                    new OnIntent(LuisConstant.CONTACT)
                    {
                        Condition = $"#{LuisConstant.CONTACT}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Contact()}")
                        },
                    },

                    new OnIntent(LuisConstant.CANCEL)
                    {
                        Condition = $"#{LuisConstant.CANCEL}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Cancel()}")                       
                        },
                    },

                    // Respond to user on message activity
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog>
                        {
                            new SendActivity("${NoIntention()}"),
                            new SendActivity("${WelcomeActions()}")
                        } 
                    },
                };
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${Welcome()}"),
                                new SendActivity("${WelcomeActions()}")
                            }
                        }
                    }
                }
            };

        }

        private static Recognizer CreateCrossTrainedRecognizer(IConfiguration configuration)
        {
            return new CrossTrainedRecognizerSet()
            {
                Recognizers = new List<Recognizer>()
                {
                    CreateLuisRecognizer(configuration),
                    //CreateQnAMakerRecognizer(configuration)
                }
            };
        }

        private static Recognizer CreateQnAMakerRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["qna:AddToDoDialog_en_us_qna"]) || string.IsNullOrEmpty(configuration["QnAHostName"]) || string.IsNullOrEmpty(configuration["QnAEndpointKey"]))
            {
                throw new Exception("NOTE: QnA Maker is not configured for AddToDoDialog. Please follow instructions in README.md to add 'qnamaker:AddToDoDialog_en_us_qna', 'QnAHostName' and 'QnAEndpointKey' to the appsettings.json file.");
            }

            return new QnAMakerRecognizer()
            {
                HostName = configuration["QnAHostName"],
                EndpointKey = configuration["QnAEndpointKey"],
                KnowledgeBaseId = configuration["qna:AddToDoDialog_en_us_qna"],

                // property path that holds qna context
                Context = "dialog.qnaContext",

                // Property path where previous qna id is set. This is required to have multi-turn QnA working.
                QnAId = "turn.qnaIdFromPrompt",

                // Disable teletry logging
                LogPersonalInformation = false,

                // Enable to automatically including dialog name as meta data filter on calls to QnA Maker.
                IncludeDialogNameInMetadata = true,

                // Id needs to be QnA_<dialogName> for cross-trained recognizer to work.
                Id = $"QnA_{nameof(RootDialog)}"
            };
        }

        private static Recognizer CreateLuisRecognizer(IConfiguration Configuration)
        {
            if (string.IsNullOrEmpty(Configuration["Luis:LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["Luis:LuisAPIHostName"]))
            {
                throw new Exception("Your AddToDoDialog's LUIS application is not configured for AddToDoDialog. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["Luis:LuisAPIHostName"],
                EndpointKey = Configuration["Luis:LuisAPIKey"],
                ApplicationId = Configuration["Luis:AppID"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(RootDialog)}"
            };
        }
    }
}
  