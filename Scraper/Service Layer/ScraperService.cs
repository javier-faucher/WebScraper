

using Scraper.Data_Layer;
using Scraper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Scraper.Service_Layer
{
    public class ScraperService
    {
        public ScraperContext ctx;
        public ScraperService(ScraperContext _ctx)
        {
            ctx = _ctx;
            resultRepo = new ScraperSingleResultRepo(ctx);
            sessionRepo = new SessionRepo(ctx);
        }
        private ScraperSingleResultRepo resultRepo;

        private SessionRepo sessionRepo;

        /// <summary>
        ///Entry point to deal with  the check request for the controller. 
        /// This method takes in the request body from the controller creates the URL for the corresponding google 
        /// Processes the response then returns a Dictionary containing:
        /// The key words for the google search and the query/ URl from the body
        /// The google search URL 
        /// An array indicating where the key words appeared in the search 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Dictionary<string, object> check(requestBody request)
        {
            List<int> appearancesInSearch = new List<int>();
            List<string> urlList = new List<string>();
            List<string> rawResultsList;

            string requestURL = createURL(request.keyWords, request.numOfResults);
            String rawhtml = getPage(requestURL);

            string formattedhtml = formatPage(rawhtml);
            rawResultsList = seperateResult(formattedhtml, request.numOfResults);


            for (int i = 0; i < request.numOfResults; i++)
            {
                string resultUrl = getURL(rawResultsList[i]);
                urlList.Add(resultUrl);
                if (resultUrl.Trim().Contains(request.query.Trim()) || resultUrl.Trim() == request.query.Trim())
                {
                    appearancesInSearch.Add(i + 1);
                }
            }

            Session session = saveSession(request, requestURL, appearancesInSearch.ToArray());
            saveResults(session, urlList.ToArray());

            Dictionary<string, object> response = new Dictionary<string, object>();
            response.Add("appeared", appearancesInSearch);
            response.Add("requestedURL", requestURL);
            response.Add("query", request.query);
            response.Add("keyWords", request.keyWords);

            return response;

        }
        /// <summary>
        /// Takes in the parameters necessary to create a valid Session Model
        /// Converts the array contaning the position of appearances in the search to a strin g
        /// calls the corresponding repo to save the session to the database
        /// </summary>
        /// <returns></returns>
        public Session saveSession(requestBody request, string requestedUrl, int[] appearances)
        {
            string appearancesString = "";
            for (int i = 0; i < appearances.Length; i++)
            {
                if (i + 1 != appearances.Length)
                {
                    appearancesString += appearances[i].ToString() + ",";
                }
                else
                {
                    appearancesString += appearances[i].ToString();
                }
            }
            Session session = new Session()
            {
                requestTime = DateTime.Now,
                keyWords = request.keyWords,
                requestedUrl = requestedUrl,
                query = request.query,
                appearedList = appearancesString,
                numberOfResults = request.numOfResults
            };

            sessionRepo.saveSession(session);

            return session;
        }
        /// <summary>
        /// Generates the google search URL from the key words and the num of request
        /// </summary>
        /// <returns></returns>
        public string createURL(string keyWords, int numOfRequests)
        {
            string url = "https://www.google.co.uk/search?num=";
            url += numOfRequests.ToString() + "&q=";
            string[] paramList = keyWords.Trim().Split(' ');
            for (int i = 0; i < paramList.Length; i++)
            {
                if (i + 1 < paramList.Length)
                {
                    url += Uri.EscapeDataString(paramList[i]) + '+';
                }
                else
                {
                    url += Uri.EscapeDataString(paramList[i]);
                }
            }
            return url;
            // "https://www.google.co.uk/search?num=5&q=land+registry+search",

        }
        /// <summary>
        /// Saves the all the results linked to a single session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="urls"></param>
        public void saveResults(Session session, string[] urls)
        {
            foreach (string url in urls)
            {
                singleResult result = new singleResult()
                {
                    url = url,
                    session = session
                };
                resultRepo.saveResult(result);
            }

        }

        /// <summary>
        /// Retrieves the given page from the provided Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string getPage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            var response = request.GetResponse();
            String responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
        /// <summary>
        /// Removes the all script, style, header and head components from the page 
        /// They are unecessary 
        /// </summary>
        /// <param name="unformatted"></param>
        /// <returns></returns>
        public string formatPage(string unformatted)
        {
            string formatted = unformatted;
            // remove style ,script and header components
            formatted = removeComponents(formatted, "script");
            formatted = removeComponents(formatted, "style");
            if (formatted.Contains("<header"))
            {
                formatted = removeComponents(formatted, "header");
            }
            formatted = removeComponents(formatted, "head");

            return formatted;
        }
        /// <summary>
        /// Takes in a DOM component and the html and removes all references to that DOm 
        /// </summary>
        /// <param name="unformatted"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public string removeComponents(string unformatted, string toRemove)
        {
            string formatted;
            int currentIndex = 0;
            bool found = false;
            int i = 0;

            do
            {
                int startIndex = unformatted.IndexOf("<" + toRemove, currentIndex);
                int endIndex = unformatted.IndexOf("</" + toRemove + ">", startIndex);
                int count = endIndex + toRemove.Length + 3 - startIndex;
                unformatted = unformatted.Remove(startIndex, count);
                currentIndex = endIndex + toRemove.Length - count;
                i++;
            } while (unformatted.IndexOf("<" + toRemove, currentIndex) != -1);

            return unformatted;


        }

        /// <summary>
        /// Seperates out the individual search results components from the page and returns athem as a list of strings
        /// </summary>
        /// <param name="page"></param>
        /// <param name="numOfRequests"></param>
        /// <returns></returns>

        public List<string> seperateResult(string page, int numOfRequests)
        {
            List<string> resultList = new List<string>();

            bool found = false;
            int currentIndex = 0;
            string resultDomStart = "<div><div class=\"ZINbbc";
            string resultsEnd = "</div></div><footer>";

            for (int i = 0; i < numOfRequests; i++)
            {
                int startIndex = page.IndexOf(resultDomStart, currentIndex);
                int nextIndex;
                if (i + 1 == numOfRequests)
                {
                    nextIndex = page.IndexOf(resultsEnd, startIndex + resultDomStart.Length);
                }
                else
                {
                    nextIndex = page.IndexOf(resultDomStart, startIndex + resultDomStart.Length);
                }
                resultList.Add(page.Substring(startIndex, nextIndex - startIndex));
                currentIndex = nextIndex;
            }


            return resultList;
        }

        public int getNumOfResultsRequested(string Url)
        {
            try
            {
                int indexStart = Url.IndexOf("num=") + 4;
                int indexEnd = Url.IndexOf("&", indexStart);
                return int.Parse(Url.Substring(indexStart, indexEnd - indexStart));
            }
            catch
            {
                return 10;

            }

        }
        /// <summary>
        /// For google searcher gets the Url from the div 
        /// </summary>
        /// <param name="singleResultHtml"></param>
        /// <returns></returns>
        public string getURL(string singleResultHtml)
        {

            string URLDiv = getURLDiv(singleResultHtml);

            int indexUrlStart = URLDiv.IndexOf('>');
            int indexUrlEnd = URLDiv.IndexOf(' ', indexUrlStart);
            return URLDiv.Substring(indexUrlStart + 1);
        }
        /// <summary>
        /// Gets the div containing the URl from the rest of the of the html for a single result
        /// </summary>
        /// <param name="singleResultHtml"></param>
        /// <returns></returns>
        public string getURLDiv(string singleResultHtml)
        {
            int indexDivStart = 0;
            for (int i = 0; i < 5; i++)
            {
                int tempIndex = singleResultHtml.IndexOf("<div", indexDivStart);
                indexDivStart = tempIndex + 4;
            }
            int indexDivEnd = singleResultHtml.IndexOf("</div>", indexDivStart);

            return singleResultHtml.Substring(indexDivStart, indexDivEnd - indexDivStart);
        }
    }

}