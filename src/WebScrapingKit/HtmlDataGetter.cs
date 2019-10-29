using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScrapingKit
{
    public class HtmlDataGetter
    {
        #region プロパティ

        public bool IsCollecting { get; private set; } = false;

        #endregion

        #region データメンバ

        // 連続アクセスするときの時間間隔[msec]
        private int _sleepInterval = 2000;

        // 連続アクセスでデータ取得に失敗したときの時間間隔[msec]
        private int _sleepIntervalWhenError = 200;

        #endregion

        #region Publicメソッド

        public string[] GetDataFromHtml(string url, string xpath)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(xpath)) throw new ArgumentNullException("xpath");

            var doc = new HtmlDocument();
            var web = new WebClient();
            web.Encoding = Encoding.UTF8;
            try
            {
                doc.LoadHtml(web.DownloadString(url));
            }
            catch (Exception ex)
            {
                throw new Exception("GetHtmlメソッドで，HTML取得時にエラーが発生しました。", ex);
            }

            var targetCollection = doc.DocumentNode.SelectNodes(xpath);
            if (targetCollection == null)
            {
                throw new MissingXPathException("GetHtmlメソッドで，指定したxpathに該当するHTML要素が見つかりませんでした。");
            }

            string[] output = GetInnerTexts(targetCollection);
            return output;
        }

        public string[] GetAttributeFromHtml(string url, string xpath, string attributeName)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(xpath)) throw new ArgumentNullException("xpath");
            if (string.IsNullOrEmpty(attributeName)) throw new ArgumentNullException("attributeName");

            var doc = new HtmlDocument();
            var web = new WebClient();
            web.Encoding = Encoding.UTF8;
            try
            {
                doc.LoadHtml(web.DownloadString(url));
            }
            catch (Exception ex)
            {
                throw new Exception("GetHtmlメソッドで，HTML取得時にエラーが発生しました。", ex);
            }

            var targetCollection = doc.DocumentNode.SelectNodes(xpath);
            if (targetCollection == null)
            {
                throw new MissingXPathException("GetHtmlメソッドで，指定したxpathに該当するHTML要素が見つかりませんでした。");
            }

            string[] output = GetAttributes(targetCollection, attributeName);
            return output;
        }

        //public Task GetDataFromHtmlAsync(string url, string xpath, int idFrom, int idTo, string urlFooter, IProgress<HtmlData> progress, CancellationToken cancelToken)
        //{
        //    if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
        //    if (string.IsNullOrEmpty(xpath)) throw new ArgumentNullException("xpath");
        //    if (string.IsNullOrEmpty(urlFooter)) throw new ArgumentNullException("urlFooter");
        //    if (progress == null) throw new ArgumentNullException("progress");
        //    if (cancelToken == null) throw new ArgumentNullException("cancelToken");

        //    return Task.Run(() =>
        //    {
        //        GetDataFromHtml(url, xpath, idFrom, idTo, urlFooter, progress, cancelToken);
        //    });
        //}

        public Task GetDataFromHtmlAsync(string[] urls, string xpath, IProgress<HtmlData> progress, CancellationToken cancelToken)
        {
            if (urls == null) throw new ArgumentNullException("urls");
            if (urls.Length == 0) throw new ArgumentException("urls.Length");
            if (string.IsNullOrEmpty(xpath)) throw new ArgumentNullException("xpath");
            if (progress == null) throw new ArgumentNullException("progress");
            if (cancelToken == null) throw new ArgumentNullException("cancelToken");

            return Task.Run(() =>
            {
                GetDataFromHtml(urls, xpath, progress, cancelToken);
            });
        }

        public Task GetAttributeFromHtmlAsync(string[] urls, string xpath, string attributeName, IProgress<HtmlData> progress, CancellationToken cancelToken)
        {
            if (urls == null) throw new ArgumentNullException("urls");
            if (urls.Length == 0) throw new ArgumentException("urls.Length");
            if (string.IsNullOrEmpty(xpath)) throw new ArgumentNullException("xpath");
            if (progress == null) throw new ArgumentNullException("progress");
            if (cancelToken == null) throw new ArgumentNullException("cancelToken");

            return Task.Run(() =>
            {
                GetAttributeFromHtml(urls, xpath, attributeName, progress, cancelToken);
            });
        }

        public HtmlDataGetter(int sleepInterval = 2000, int sleepIntervalWhenError = 200)
        {
            if (sleepInterval < 1) throw new ArgumentOutOfRangeException("sleepInterval");
            if (sleepIntervalWhenError < 1) throw new ArgumentOutOfRangeException("sleepIntervalWhenError");

            _sleepInterval = sleepInterval;
            _sleepIntervalWhenError = sleepIntervalWhenError;
        }

        #endregion

        #region Privateメソッド

        //private void GetDataFromHtml(string url, string xpath, int idFrom, int idTo, string urlFooter, IProgress<HtmlData> progress, CancellationToken cancelToken)
        //{
        //    IsCollecting = true;
        //    var doc = new HtmlDocument();
        //    var web = new WebClient();
        //    web.Encoding = Encoding.UTF8;
        //    for (int id = idFrom; id <= idTo && IsCollecting; id++)
        //    {
        //        string wholeUrl = url + id.ToString() + urlFooter;
        //        try
        //        {
        //            doc.LoadHtml(web.DownloadString(wholeUrl));
        //        }
        //        catch
        //        {
        //            // データが取得できなかった旨を呼び出し元に通知する。
        //            progress.Report(new HtmlData(
        //                null,
        //                0,
        //                null,
        //                HtmlDataGetterResult.LoadHtmlFailed
        //                ));
        //            Cancellation(cancelToken);
        //            Thread.Sleep(_sleepIntervalWhenError);
        //            continue;
        //        }

        //        //指定したXPathをもとに文を取得する。
        //        var targetCollection = doc.DocumentNode.SelectNodes(xpath);

        //        // データが得られなかった場合
        //        if (targetCollection == null)
        //        {
        //            // データが取得できなかった旨を呼び出し元に通知する。
        //            progress.Report(new HtmlData(
        //                null,
        //                0,
        //                null,
        //                HtmlDataGetterResult.DataNotFound
        //                ));
        //            Cancellation(cancelToken);
        //            Thread.Sleep(_sleepIntervalWhenError);
        //            continue;
        //        }

        //        string[] data = new string[targetCollection.Count];

        //        try
        //        {
        //            data = GetInnerTexts(targetCollection);
        //        }
        //        catch
        //        {
        //            // データの取得に失敗した場合
        //            // データが取得できなかった旨を呼び出し元に通知する。
        //            progress.Report(new HtmlData(
        //                null,
        //                0,
        //                null,
        //                HtmlDataGetterResult.GetInnerTextFailed
        //                ));
        //            Cancellation(cancelToken);
        //            Thread.Sleep(_sleepIntervalWhenError);
        //            continue;
        //        }

        //        // 取得した情報を呼び出し元に返す。
        //        progress.Report(new HtmlData(
        //            wholeUrl,
        //            id,
        //            data,
        //            id == idTo ? HtmlDataGetterResult.FinalDataSuccess : HtmlDataGetterResult.Success
        //            ));

        //        Cancellation(cancelToken);
        //        Thread.Sleep(_sleepInterval);
        //    }
        //    IsCollecting = false;
        //}

        private void GetDataFromHtml(string[] urls, string xpath, IProgress<HtmlData> progress, CancellationToken cancelToken)
        {
            IsCollecting = true;
            var doc = new HtmlDocument();
            var web = new WebClient();
            web.Encoding = Encoding.UTF8;
            for (int i = 0; i < urls.Length; i++)
            {
                if (!IsCollecting) break;

                string url = urls[i];

                try
                {
                    doc.LoadHtml(web.DownloadString(url));
                }
                catch
                {
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.LoadHtmlFailed
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                //指定したXPathをもとに文を取得する。
                var targetCollection = doc.DocumentNode.SelectNodes(xpath);

                // データが得られなかった場合
                if (targetCollection == null)
                {
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.DataNotFound
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                string[] data = new string[targetCollection.Count];

                try
                {
                    data = GetInnerTexts(targetCollection);
                }
                catch
                {
                    // データの取得に失敗した場合
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.GetInnerTextFailed
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                // 取得した情報を呼び出し元に返す。
                progress.Report(new HtmlData(
                    url,
                    -1,
                    data,
                    i == urls.Length - 1 ? HtmlDataGetterResult.FinalDataSuccess : HtmlDataGetterResult.Success
                    ));

                Cancellation(cancelToken);
                Thread.Sleep(_sleepInterval);
            }
            IsCollecting = false;
        }

        private void GetAttributeFromHtml(string[] urls, string xpath, string attributeName, IProgress<HtmlData> progress, CancellationToken cancelToken)
        {
            IsCollecting = true;
            var doc = new HtmlDocument();
            var web = new WebClient();
            web.Encoding = Encoding.UTF8;
            for (int i = 0; i < urls.Length; i++)
            {
                if (!IsCollecting) break;

                string url = urls[i];

                try
                {
                    doc.LoadHtml(web.DownloadString(url));
                }
                catch
                {
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.LoadHtmlFailed
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                //指定したXPathをもとに文を取得する。
                var targetCollection = doc.DocumentNode.SelectNodes(xpath);

                // データが得られなかった場合
                if (targetCollection == null)
                {
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.DataNotFound
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                string[] data = new string[targetCollection.Count];

                try
                {
                    data = GetAttributes(targetCollection, attributeName);
                }
                catch
                {
                    // データの取得に失敗した場合
                    // データが取得できなかった旨を呼び出し元に通知する。
                    progress.Report(new HtmlData(
                        null,
                        0,
                        null,
                        HtmlDataGetterResult.GetAttributeFailed
                        ));
                    Cancellation(cancelToken);
                    Thread.Sleep(_sleepIntervalWhenError);
                    continue;
                }

                // 取得した情報を呼び出し元に返す。
                progress.Report(new HtmlData(
                    url,
                    -1,
                    data,
                    i == urls.Length - 1 ? HtmlDataGetterResult.FinalDataSuccess : HtmlDataGetterResult.Success
                    ));

                Cancellation(cancelToken);
                Thread.Sleep(_sleepInterval);
            }
            IsCollecting = false;
        }

        private void Cancellation(CancellationToken cancelToken)
        {
            try
            {
                // キャンセル要求があれば例外を発生させタスクを終了させる.
                cancelToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                IsCollecting = false;
            }
        }

        private string[] GetInnerTexts(HtmlNodeCollection c)
        {
            string[] output = new string[c.Count];
            for (int i = 0; i < c.Count; i++)
            {
                output[i] = c[i].InnerText.Replace("\r", "").Replace("\n", "");
            }

            return output;
        }

        private string[] GetAttributes(HtmlNodeCollection c, string attributeName)
        {
            string[] output = new string[c.Count];
            for (int i = 0; i < c.Count; i++)
            {
                string attributeValue = c[i].GetAttributeValue(attributeName, null);
                output[i] = attributeValue;
            }

            return output;
        }

        #endregion
    }
}
