namespace CV_Chatbot.Constants
{
    public static class LuisConstant
    {
        public const string WELCOME = "WELCOME";
        public const string STUDIES = "STUDIES";
        public const string CANCEL = "CANCEL";
        public const string CONTACT = "CONTACT";
        public const string EXPERIENCE = "EXPERIENCE";
        public const string BACKEND = "BACKEND";
        public const string FRONTEND = "FRONTEND";
        public const string CLOUD = "CLOUD";
        public const string EMAIL = "EMAIL";
        public const string BYE = "BYE";
        public const string BACK = "BACK"; 
    }
}

/*
 * Welcome => Presentación de mi persona, los invito a hacer preguntas para conocerme. (Acciones sugeridas: estudios, contacto, experiencia).
 * Estudios => Hago un resumen de mis estudios, secundaria, universidad.(nuevo dialogo) (Poner una pregunta para materias que faltan?, 
 *  titulos otorgados (tecnico, licenciado?), cursos, open hack, curso de azure fundamentals)
 * Cancel => salir, quiero salir, me voy, saludos. => Agradezco por hablar con el bot, dejo mi mail para cualquier comunicación.
 * Adios => capaz es mejor poner aca el mail para cualquier contacto.
 * Contacto => (nuevo dialogo?) presentar linkedin, mail, adaptive card con un cuerpo que permita enviar el mail mediante SendGrid
 * Experiencia => (nuevo dialogo?)Hablo un poco de como empece mi trabajo en softtek, academia orientada a c#. Distintos problemas, menciono las tecnologias.
 *  (Intencion por tecnologia, React, c# dotnet .net asp net, wep api, servicios, azure, cloud, sql).
 */