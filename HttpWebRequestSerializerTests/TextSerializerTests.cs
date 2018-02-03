using System;
using HttpWebRequestSerializer;
using HttpWebRequestSerializer.Extensions;
using NUnit.Framework;

namespace HttpWebRequestSerializerTests
{
    [TestFixture()]
    public class TextSerializerTests
    {
        private const string sampleGet =
            @"GET https://httpbin.org/get HTTP/1.1
Host: httpbin.org
Connection: keep-alive
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36
Upgrade-Insecure-Requests: 1
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9";

        private const string serializedGet = @"{""Uri"":""https://httpbin.org/get"",""Headers"":{""Method"":""GET"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""Connection"":""keep-alive"",""User-Agent"":""Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36"",""Upgrade-Insecure-Requests"":""1"",""Accept"":""text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"",""Accept-Encoding"":""gzip, deflate, br"",""Accept-Language"":""en-US,en;q=0.9""},""Cookie"":{},""RequestData"":null}";

        private const string samplePost =
            @"POST https://httpbin.org/post HTTP/1.1
Host: httpbin.org
User-Agent: curl/7.54.1
Accept: */*
Content-Length: 10
Content-Type: application/x-www-form-urlencoded
Cookie: ilikecookies=chocchip

helloworld";

        private const string sampleRequestWithExpect =
            @"POST http://www.example.com/ HTTP/1.1
User-Agent: curl/7.54.1
Accept: */*
Content-Type: application/x-www-form-urlencoded
Host: www.example.com
Content-Length: 10
Expect: 100-continue
Connection: Keep-Alive

helloworld";

        private const string samplePostNoCarriageReturn = @"POST https://httpbin.org/post HTTP/1.1\nHost: httpbin.org\nUser-Agent: curl/7.54.1\nAccept: */*\nContent-Length: 10\nContent-Type: application/x-www-form-urlencoded\nCookie: ilikecookies=chocchip\n\nhelloworld";

        private const string serializedPost = @"{""Uri"":""https://httpbin.org/post"",""Headers"":{""Method"":""POST"",""HttpVersion"":""HTTP/1.1"",""Host"":""httpbin.org"",""User-Agent"":""curl/7.54.1"",""Accept"":""*/*"",""Content-Length"":""10"",""Content-Type"":""application/x-www-form-urlencoded""},""Cookie"":{""ilikecookies"":""chocchip""},""RequestData"":""helloworld""}";
        private const string serializedPostNoCookieNoData = "{\"Uri\":\"https://httpbin.org/post\",\"Headers\":{\"Method\":\"POST\",\"HttpVersion\":\"HTTP/1.1\",\"Host\":\"httpbin.org\",\"User-Agent\":\"curl/7.54.1\",\"Accept\":\"*/*\",\"Content-Length\":\"10\",\"Content-Type\":\"application/x-www-form-urlencoded\"}}";

        [Test]
        public void Should_Serlialize_Raw_Request_Get()
        {
            var json = HttpParser.GetRawRequestAsJson(sampleGet);
            Assert.AreEqual(serializedGet, json);
            Console.WriteLine(json);
        }

        [TestCase(samplePost, serializedPost)]
        [TestCase(samplePostNoCarriageReturn, serializedPost)]
        public void Should_Serlialize_Raw_Request_Post(string sample, string expected)
        {
            var json = HttpParser.GetRawRequestAsJson(sample);
            Assert.AreEqual(expected, json);
        }

        [Test]
        public void Should_Not_Serialize_Data_Or_Cookies()
        {
            var so = new SerializationOptions(new [] { SerializationOptionKey.Cookie });
            so.IgnoreKey(SerializationOptionKey.Cookie);
            so.IgnoreKey(SerializationOptionKey.RequestData);
            var json = HttpParser.GetRawRequestAsJson(samplePost, so);

            Assert.AreEqual(serializedPostNoCookieNoData, json);
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Get()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedGet);
            Assert.AreEqual("System.Net.HttpWebRequest", req.GetType().ToString());
        }

        [Test]
        public void Should_Build_Http_Request_From_Json_Post()
        {
            var req = RequestBuilder.CreateWebRequestFromJson(serializedPost);
            Assert.AreEqual("System.Net.HttpWebRequest", req.GetType().ToString());
        }

        [Test, Ignore("Requires Web")]
        public void Should_Build_ParsedRequestGet()
        {
            var pr = HttpParser.GetParsedRequest(sampleGet);
            var req = pr.CreateWebRequestFromParsedRequest();

            req.GetResponseString();
        }

        [Test, Ignore("Requires Web")]
        public void Should_Build_ParsedRequestPost()
        {
            var pr = HttpParser.GetParsedRequest(samplePost);
            var req = pr.CreateWebRequestFromParsedRequest();

            req.GetResponseString();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("gibberish \n falksfjl;")]
        public void Should_Handle_Invalid_Request(string input)
        {
            Assert.Throws<Exception>(() => HttpParser.GetRawRequestAsJson(input));
        }

        [Test]
        public void Should_Substitute_Url()
        {
            var rawWithDelimiter = @"GET https://www.wellmark.com/ProviderFinder/Doctor/DoctorPartial?#{{QueryString}} HTTP/1.1
Host: www.wellmark.com
Connection: keep-alive
Accept: */*
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36
DNT: 1
Referer: https://www.wellmark.com/ProviderFinder/Doctor/Results
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.8";

            var expectedRaw = @"GET https://www.wellmark.com/ProviderFinder/Doctor/DoctorPartial?p=2&lat=41.6&lng=-93.61&rad=10&n=WBLUEPPO&s=P0HI0X41MY&if1=1&o=asc&ps=10&distance=2&mode=b&nn=Wellmark%20Blue%20PPO&sn=Family%20Practice&av=50301&DirectoryNetwork=WBLUEPPO HTTP/1.1
Host: www.wellmark.com
Connection: keep-alive
Accept: */*
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36
DNT: 1
Referer: https://www.wellmark.com/ProviderFinder/Doctor/Results
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.8";
            var queryString = @"p=2&lat=41.6&lng=-93.61&rad=10&n=WBLUEPPO&s=P0HI0X41MY&if1=1&o=asc&ps=10&distance=2&mode=b&nn=Wellmark%20Blue%20PPO&sn=Family%20Practice&av=50301&DirectoryNetwork=WBLUEPPO";

            var actualRaw = HttpWebRequestSerializer.Extensions.StringExtensions.SubstituteText(rawWithDelimiter, queryString, @"#{{(?<QueryString>\w+)}}");

            Assert.AreEqual(expectedRaw, actualRaw);
        }

        [Test]
        public void S()
        {
            var r = @"GET http://www.aetna.com/dse/search/results?searchQuery=Pediatrics&geoSearch=90210&pagination.offset=&zipCode=90210&distance=0&filterValues=&useZipForLatLonSearch=true&fastProduct=&currentSelectedPlan=&selectedMemberForZip=&sessionCachingKey=&loggedInZip=true&modalSelectedPlan=BHHMO&isTab1Clicked=&isTab2Clicked=&quickSearchTypeMainTypeAhead=&quickSearchTypeThrCol=byProvType&mainTypeAheadSelectionVal=&thrdColSelectedVal=Pediatrics&isMultiSpecSelected=&hospitalNavigator=&productPlanName=(CA)+Aetna+Basic+HMO&hospitalNameFromDetails=&planCodeFromDetails=&hospitalFromDetails=false&aetnaId=&Quicklastname=&Quickfirstname=&QuickZipcode=90210&QuickCoordinates=34.10313099999999%2C-118.41625300000001&quickSearchTerm=&ipaFromDetails=&ipaFromResults=&ipaNameForProvider=&porgId=&officeLocation=&otherOfficeProviderName=&officesLinkIsTrueDetails=false&groupnavigator=&groupFromDetails=&groupFromResults=&groupNameForProvider=&suppressFASTCall=&classificationLimit=DMP&suppressFASTDocCall=false&axcelSpecialtyAddCatTierTrueInd=&suppressHLCall=&pcpSearchIndicator=true&specSearchIndicator=&stateCode=CA&geoMainTypeAheadLastQuickSelectedVal=90210&geoBoxSearch=true&lastPageTravVal=&debugInfo=&debugInfoMedicare=&planCategoryVal=&stateCodeFromCounty=&countyCode=&planYear=&linkwithoutplan=&site_id=dse&langPref=en&sendZipLimitInd=&ioeqSelectionInd=&ioe_qType=&sortOrder=&healthLineErrorMessage=&QuickGeoType=&originalWhereBoxVal=&quickCategoryCode=&comServletUrlWithParms=http%3A%2F%2Fwww30.aetna.com%2Fcom%2Forgmapping%2FCOMServlet%3FrequestoruniqueKey%3Dhttp%253A%252F%252Fwww.aetna.com%252Fdse%252Fsearch%253Fsite_id%253Ddse%26keyType%3DURL%26commPurpose%3DDSE%26callingPage%3Dcall_servlet.html&orgId=39934088&orgArrangementid=19200955&productPlanPipeName=&quickZipCodeFromFirstHLCall=&quickStateCodeFromFirstHLCall= HTTP/1.1
Host: www.aetna.com
Connection: keep-alive
Accept: */*
X-Requested-With: XMLHttpRequest
ADRUM: isAjax:true
X-Distil-Ajax: vsrfycewbxcddysuvtett
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36
Referer: http://www.aetna.com/dse/search?site_id=dse&langPref=en
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9";

            var json = HttpParser.GetRawRequestAsJson(r);
        }

        [Test]
        public void Should_Parse_Get_No_Query_String()
        {
            var r = @"GET http://www.trdpnetwork.org/FGP/Provider HTTP/1.1";

            var so = new SerializationOptions();

            var json = HttpParser.GetRawRequestAsJson(r, so);
            var req = RequestBuilder.CreateWebRequestFromJson(json);

            Console.WriteLine(json);
        }
    }
}