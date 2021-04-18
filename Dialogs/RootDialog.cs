using System;
using System.Collections.Generic;
using CV_Chatbot.Constants;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA.Recognizers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Extensions.Configuration;

namespace CV_Chatbot.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly IConfiguration _configuration;

        public RootDialog(IConfiguration configuration) : base(nameof(RootDialog))
        {
            _configuration = configuration;
            Recognizer = CreateCrossTrainedRecognizer(configuration);
            // These steps are executed when this Adaptive Dialog begins

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
                            new SendActivity("Una intención de estudio")
                        },

                    },

                    new OnIntent(LuisConstant.WELCOME)
                    {
                        Condition = $"#{LuisConstant.WELCOME}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Una intención de bienvenida")
                        },
                    },

                    new OnIntent(LuisConstant.EXPERIENCE)
                    {
                        Condition = $"#{LuisConstant.EXPERIENCE}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Una intención de experiencia")
                        },
                    },

                    new OnIntent(LuisConstant.CONTACT)
                    {
                        Condition = $"#{LuisConstant.CONTACT}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Una intención de contacto")
                        },
                    },

                    new OnIntent(LuisConstant.CANCEL)
                    {
                        Condition = $"#{LuisConstant.CANCEL}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Una intención de cancelar")                        },
                    },

                    // Respond to user on message activity
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog>
                        {
                            new SendActivity("No soy ninguna intención")
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
                                new SendActivity("Hola! Bienvenido a mi chatbot, un lugar donde podes conocerme hablando.")
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
  