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
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using CV_Chatbot.Constants;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder;
using CV_Chatbot.Services;

namespace CV_Chatbot.Dialogs
{
    public class RootDialog : ComponentDialog
    {
        private readonly IConfiguration _configuration;
        private AdaptiveDialog _rootDialog;
        private readonly IEmailService _emailService;

        public RootDialog(IConfiguration configuration, IEmailService emailService) : base(nameof(RootDialog))
        {
            _configuration = configuration;
            _emailService = emailService;
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);

            this._rootDialog = new AdaptiveDialog(nameof(RootDialog))
            {
                Recognizer = CreateCrossTrainedRecognizer(configuration),
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                Triggers = new List<OnCondition>()
                {
                    new OnDialogEvent("webchat/join") { Actions = WelcomeUserSteps()  },
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity() { Actions = WelcomeUserSteps() },

                    new OnIntent(LuisConstant.STUDIES)
                    {
                        Condition = $"#{LuisConstant.STUDIES}.score >= 0.8",
                        Actions = new List<Dialog>() { new SendActivity("${Studies()}"), new SendActivity("${WelcomeActions()}") }
                    },

                    new OnIntent(LuisConstant.WELCOME)
                    {
                        Condition = $"#{LuisConstant.WELCOME}.score >=0.8",
                        Actions = new List<Dialog>() { new SendActivity("${WelcomeRetry()}"), new SendActivity("${WelcomeActions()}") }
                    },

                    new OnIntent(LuisConstant.EXPERIENCE)
                    {// habra que poner un nuevo app id?
                        Condition = $"#{LuisConstant.EXPERIENCE}.score >=0.8",
                        Actions = new List<Dialog>() {  new BeginDialog(nameof(ExperienceDialog)) }
                    },

                    new OnIntent(LuisConstant.CONTACT)
                    {
                        Condition = $"#{LuisConstant.CONTACT}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Contact()}"),
                            new SendActivity("${ContactCard()}"),
                            new SendActivity("${WelcomeActions()}"),
                        }
                    },

                    new OnIntent(LuisConstant.EMAIL)
                    {
                        Condition = $"#{LuisConstant.EMAIL}.score >=0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${Email()}"),
                            new SendActivity("${EmailCard()}"),
                        }
                    },

                   new OnIntent("None")
                    {
                        Condition = $"turn.activity.value.body != null",
                        Actions = new List<Dialog>()
                        {
                            new CodeAction(async (dc, options) =>
                            {
                                dynamic value = dc.Context.Activity.Value;
                                string body = (string)value?.body;
                                string title = (string)value?.title;
                                bool success = await _emailService.SendEmail(title, body);
                                if(success)
                                    await dc.Context.SendActivityAsync(MessageFactory.Text("Gracias por contactarte!"));
                                else
                                  await dc.Context.SendActivityAsync(MessageFactory.Text("Lo siento, ocurrio un problema al intentar enviar el email."));

                                return await dc.EndDialogAsync();
                            }),
                        }
                    },

                    new OnIntent(LuisConstant.BYE)
                    {
                        Condition = $"#{LuisConstant.BYE}.score >=0.8",
                        Actions = new List<Dialog>() { new SendActivity("Bye") }
                    },

                    new OnIntent(LuisConstant.CANCEL)
                    {
                        Condition = $"#{LuisConstant.CANCEL}.score >=0.8",
                        Actions = new List<Dialog>() { new SendActivity("${Cancel()}") },
                    },


                    // Respond to user on message activity
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog> { new SendActivity("${NoIntention()}"), new SendActivity("${WelcomeActions()}") }
                    },
                }
            };

            this.AddDialog(this._rootDialog);
            // Add all child dialogS
            this.AddDialog(new ExperienceDialog(configuration));

            // The initial child Dialog to run.
            this.InitialDialogId = nameof(RootDialog);

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
                }
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
                ApplicationId = Configuration["Luis:RootAppID"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(RootDialog)}"
            };
        }
    }
}
  