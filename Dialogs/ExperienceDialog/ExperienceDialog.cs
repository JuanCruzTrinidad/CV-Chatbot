using CV_Chatbot.Constants;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace CV_Chatbot.Dialogs
{
    public class ExperienceDialog : ComponentDialog
    {
        private readonly IConfiguration _configuration;
        private readonly AdaptiveDialog _experienceDialog;
        public ExperienceDialog(IConfiguration configuration) : base(nameof(ExperienceDialog))
        {
            _configuration = configuration;
            string[] paths = { ".", "Dialogs", "ExperienceDialog", "ExperienceDialog.lg" };
            string fullPath = Path.Combine(paths);

            this._experienceDialog = new AdaptiveDialog(nameof(ExperienceDialog))
            {
                Recognizer = CreateCrossTrainedRecognizer(configuration),

                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() { Actions = new List<Dialog>() 
                    { 
                        new SendActivity("${Experience()}"), 
                        new SendActivity("${ExperienceActions()}") } 
                    },

                    new OnIntent(LuisConstant.BACKEND)
                    {
                        Condition = $"#{LuisConstant.BACKEND}.score >= 0.8",
                        Actions = new List<Dialog>() 
                        {
                            new SendActivity("${Backend()}"),
                            new SendActivity("${ExperienceActions()}")
                        }
                    },

                    new OnIntent(LuisConstant.FRONTEND)
                    {
                        Condition = $"#{LuisConstant.FRONTEND}.score >=0.8",
                        Actions = new List<Dialog>() 
                        { 
                            new SendActivity("${Frontend()}"),
                            new SendActivity("${ExperienceActions()}")
                        }
                    },

                    new OnIntent(LuisConstant.CLOUD)
                    {
                        Condition = $"#{LuisConstant.CLOUD}.score >=0.8",
                        Actions = new List<Dialog>() 
                        { 
                            new SendActivity("${Cloud()}"),
                            new SendActivity("${ExperienceActions()}")
                        }
                    },

                },
                AutoEndDialog = false
            };

            this.AddDialog(this._experienceDialog);
            this.InitialDialogId = nameof(ExperienceDialog);
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
                ApplicationId = Configuration["Luis:ExperienceAppID"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(ExperienceDialog)}"
            };
        }
    }
}
