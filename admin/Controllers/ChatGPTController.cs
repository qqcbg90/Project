using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace admin.Controllers
{
    public class ChatGPTController : Controller
    {
        // GET: ChatGPT
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string prompt)
        {
            // Create an instance of the OpenAI class
            OpenAIAPI openai = new OpenAIAPI("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");

            // Call the API and get a response
            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = prompt;
            completionRequest.Model = OpenAI_API.Models.Model.DavinciText;
            completionRequest.MaxTokens = 5;
            
            CompletionResult result = openai.Completions.CreateCompletionAsync(completionRequest).Result;
            string response = result.ToString();

            // Pass the response to the view
            ViewBag.Response = response;

            return View();
        }

        
       
    }
}
