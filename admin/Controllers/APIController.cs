using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Images;
using KingspModel;
using KingspModel.DataModel;
using OpenAI_API.Models;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KingspModel.DB;
using System.Linq;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using System.Data;
using admin.Filters;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using HtmlAgilityPack;
using MvcPaging;
using System.Web.UI.WebControls;
using System.Security.Policy;
using static OpenAI.GPT3.ObjectModels.Models;

namespace admin.Controllers
{
    #region 翻譯結果的類別
    public class TranslationResponse
    {
        public TranslationData data { get; set; }
    }

    public class TranslationData
    {
        public Translation[] translations { get; set; }
    }

    public class Translation
    {
        public string translation { get; set; }
    }
    #endregion

    [MinutesSelect]
    [HourSelect]
    public class APIController : BaseController
    {
        public ActionResult Index(int? page, int? defaultPage, string k,string start
            , string end, HttpPostedFileBase fileImport)
        {
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
            return View($"{NodeID}_Index");
        }
        
        #region ChatGPT
        // GET: ChatGPT
        public ActionResult ChatGPT_Index()
        {
            ViewBag.ContentTitle = "ChatGPT";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChatGPT_Index(string prompt, string action ,string[] language)
        {
            ViewBag.ContentTitle = "ChatGPT";
            ViewBag.prompt = prompt;
            string response = string.Empty;
            string lang = string.Empty;
            ViewBag.lang = language;
            if (language != null && language.Length > 0)
            {
                int i = 1;
                foreach (string lang_ in language)
                {
                    lang += i + "." + lang_;
                    i++;
                }
            }
            if (prompt != null)
            {
                OpenAIAPI _openai = new OpenAIAPI("YOUR-KEY");
                if (action == "chat")
                {

                    CompletionRequest completionRequest = new CompletionRequest
                    {
                        Prompt = "Translate this into "+lang+":\n\n" + prompt + "\n\n",
                        Model = "text-davinci-003",
                        MaxTokens = 500,
                        Temperature = 0.7,
                        TopP = 1.0,
                        FrequencyPenalty = 0.0,
                        PresencePenalty = 0.0
                    };

                    var completions = await _openai.Completions.CreateCompletionAsync(completionRequest);
                    response = completions.Completions[0].Text.Trim();
                    // 將數字標題轉換為換行符
                    response = Regex.Replace(response, @"\d\.", "_");

                    ViewBag.Response = response;
                }
                else
                {
                    var request = new ImageGenerationRequest(prompt, 1, ImageSize._256, null, ImageResponseFormat.Url);
                    var response_ = await _openai.ImageGenerations.CreateImageAsync(request);
                    var imageUrl = response_.Data[0].Url;
                    ViewBag.imageurl = imageUrl;
                }
            }
            return View("ChatGPT_Index");
        }
        #endregion

        public ActionResult SQL_Index()
        {
            ViewBag.ContentTitle = "SQL_Index";
            string password = GetPSW();
            ViewBag.password = password;

            return View();
        }
        public ActionResult Weather_Index()
        {
            ViewBag.ContentTitle = "Weather_ex";
            return View();
        }

        public ActionResult Tomato_Index()
        {
            DATA1 d1 = iDB.GetAll<DATA1>().Where(p => p.NODE_ID.Equals("TodoList") && p.CREATER.Equals(User.Identity.Name)).FirstOrDefault();
            List<string> text = new List<string>();
            foreach (var ph in d1.PARAGRAPH.OrderBy(p => p.ORDER))
            {
                text.Add(ph.CONTENT+"_"+ph.ID+"_"+ph.CONTENT1);
            }
            ViewBag.list = text;
            //番茄鐘
            ViewBag.ContentTitle = "Tomato_ex";
            ViewBag.user = User.Identity.Name;
            return View();
        }

        public async Task<ActionResult> Crawler_Index(int? page, int? defaultPage)
        {
            HttpClient _client = new HttpClient();
            var response = await _client.GetAsync("https://partystar.media/fest");
            var content = await response.Content.ReadAsStringAsync();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            var divNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='td-page-content tagdiv-type']");
            var aNodes = divNode.Descendants("a");

            ViewBag.ContentTitle = "Web Crawler";
            int _defaultPage = defaultPage.ToDefaultPaging(Function.DEFAULT_PAGE_SIZE);
            page = IsPost() ? 0 : page;
           
            var datalist = new List<DataModel>();
            foreach(var a in aNodes )
            {
                var strongNodes = a.SelectSingleNode(".//ancestor::p/strong");
                /*這一行是從目前的 a 節點開始，往上選取祖先節點中的第一個 <p> 節點，
                 * 再選取該節點中的第一個 <strong> 子節點。可以看到，這裡使用了 XPath 的 ancestor 軸和 p、strong 兩個節點名稱
                 * ，並且在前面加上了 . 和 //，表示從目前節點開始往下選取，直到符合條件的節點被找到。
                 */
                var textNode = a.SelectSingleNode(".//ancestor::p/text()[not(normalize-space()='')]");
                /*
                 * normalize-space() 函數會移除字串兩端的空格和換行符等空白字符，
                 * 而 not() 函數會將節點選擇器的結果反轉，即選擇不符合指定條件的節點。
                 * 這樣就可以得到指定節點中的所有非空文本了。
                 */
                datalist.Add(new DataModel
                {
                    CONTENT1 =a.InnerText,
                    CONTENT2 =a.GetAttributeValue("href",""),
                    CONTENT3 = strongNodes?.InnerHtml?.Trim().Replace("<br>","") ?? string.Empty,
                    CONTENT4 = textNode?.InnerText?.Trim() ?? string.Empty
                });

            }

            IPagedList<DataModel> list = datalist.ToPagedList(page.ToMvcPaging(), _defaultPage);

            return View(list);
        }

        public ActionResult Game_Index() {
            ViewBag.ContentTitle = "Game_ex";
            return View();
        }

        public ActionResult Registration_Index(string id) {
            ViewBag.ContentTitle = "Registration_System_ex";
            DataModel model = new DataModel();
            DATA1 d = iDB.GetByID<DATA1>(id);
            if (d != null)
            {
                model.ATT = d.ID;
                model.ORDER = d.ORDER;
                model.STATUS = d.STATUS;
                model.DATA_TYPE = d.DATA_TYPE;
                model.CONTENT1 = d.CONTENT1;
                model.CONTENT2 = d.CONTENT2;
                model.CONTENT3 = d.CONTENT3;
                model.CONTENT4 = d.CONTENT4;
                model.CONTENT5 = d.CONTENT5;
                model.CONTENT6 = d.CONTENT6;
                model.CONTENT7 = d.CONTENT7;
                model.CONTENT8 = d.CONTENT8;
                model.CONTENT9 = d.CONTENT9;
                model.CONTENT10 = d.CONTENT10;
                model.CONTENT11 = d.CONTENT11;
                model.CONTENT12 = d.CONTENT12;
                model.CONTENT13 = d.CONTENT13;
                model.CONTENT14 = d.CONTENT14;
                model.CONTENT15 = d.CONTENT15;


                model.AttList = d.ATTACHMENT
                    .OrderBy(p => p.CREATE_DATE)
                    .ToList(); //相關圖片
               
               
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Registration_Index(string id,string name)
        {
            ViewBag.ContentTitle = "Registration_System_ex";
            return View();
        }
    }
}
