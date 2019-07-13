using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace HtmlToPDFDemo
{
    public class HtmlToPdfHelper
    {
        public string _strHtml = "";
        public string _htmlUrl = "";
        public string _saveUrl = "";

        public HtmlToPdfHelper()
        {
            _strHtml = "<p style='color:red;text-align:center;background-color:#000000;'>Hello World!<p><div style='width:150px;height:150px;background-color:blue;'></div>";
            _htmlUrl = "https://wkhtmltopdf.org/downloads.html";
            _saveUrl = @"C:\Users\Administrator\Desktop\PDFDemo\001.pdf";

            /// 把 HTML 文本内容转换为 PDF
            //HtmlTextConvertToPdf(strHtml, @"C:\Users\Administrator\Desktop\PDFDemo\001.pdf");

            /// 把 HTML 文件转换为 PDF
            //HtmlConvertToPdf(htmlUrl, @"C:\Users\Administrator\Desktop\PDFDemo\002.pdf");
        }

        /// <summary>
        /// HTML文本内容转换为PDF
        /// </summary>
        /// <param name="strHtml">HTML文本内容</param>
        /// <param name="savePath">PDF文件保存的路径</param>
        /// <returns></returns>
        public bool HtmlTextConvertToPdf()
        {
            bool flag = false;
            try
            {
                string htmlPath = HtmlTextConvertFile(_strHtml);

                flag = HtmlConvertToPdf(htmlPath, _saveUrl);
                File.Delete(htmlPath);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// HTML转换为PDF
        /// </summary>
        /// <param name="htmlPath">可以是本地路径，也可以是网络地址</param>
        /// <param name="savePath">PDF文件保存的路径</param>
        /// <returns></returns>
        public bool HtmlConvertToPdf(string htmlPath, string savePath)
        {
            bool flag = false;
            CheckFilePath(savePath);

            ///这个路径为程序集的目录，因为我把应用程序 wkhtmltopdf.exe 放在了程序集同一个目录下
            string exePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "wkhtmltopdf.exe";
            if (!File.Exists(exePath))
            {
                throw new Exception("No application wkhtmltopdf.exe was found.");
            }

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = exePath;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.Arguments = GetArguments(htmlPath, savePath);

                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();

                ///用于查看是否返回错误信息
                //StreamReader srone = process.StandardError;
                //StreamReader srtwo = process.StandardOutput;
                //string ss1 = srone.ReadToEnd();
                //string ss2 = srtwo.ReadToEnd();
                //srone.Close();
                //srone.Dispose();
                //srtwo.Close();
                //srtwo.Dispose();

                process.Close();
                process.Dispose();

                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public void DownLoadByBrowser()
        {
            ZipOutputStream zipStream = new ZipOutputStream(File.Create(_htmlUrl));
            //if (System.IO.File.Exists(_saveUrl))
            //{
            //    FileStream fs = new FileStream(_saveUrl, FileMode.Open);
            //    byte[] bytes = new byte[(int)fs.Length];
            //    fs.Read(bytes, 0, bytes.Length);
            //    fs.Close();
            //    if (Request.UserAgent != null)
            //    {
            //        string userAgent = Request.UserAgent.ToUpper();
            //        if (userAgent.IndexOf("FIREFOX", StringComparison.Ordinal) <= 0)
            //        {
            //            Response.AddHeader("Content-Disposition",
            //                          "attachment;  filename=" + HttpUtility.UrlEncode(pdfName, Encoding.UTF8));
            //        }
            //        else
            //        {
            //            Response.AddHeader("Content-Disposition", "attachment;  filename=" + pdfName);
            //        }
            //    }
            //    Response.ContentEncoding = Encoding.UTF8;
            //    Response.ContentType = "application/octet-stream";
            //    //通知浏览器下载文件而不是打开
            //    Response.BinaryWrite(bytes);
            //    Response.Flush();
            //    Response.End();
            //    fs.Close();
            //    System.IO.File.Delete(path);
            //}
            //else
            //{
            //    Response.Write("文件未找到,可能已经被删除");
            //    Response.Flush();
            //    Response.End();
            //}
        }
    

        /// <summary>
        /// 获取命令行参数
        /// </summary>
        /// <param name="htmlPath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private string GetArguments(string htmlPath, string savePath)
        {
            if (string.IsNullOrEmpty(htmlPath))
            {
                throw new Exception("HTML local path or network address can not be empty.");
            }

            if (string.IsNullOrEmpty(savePath))
            {
                throw new Exception("The path saved by the PDF document can not be empty.");
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" --page-size A4 ");
            stringBuilder.Append(" --page-height 297 ");        //页面高度100mm
            stringBuilder.Append(" --page-width 210 ");         //页面宽度100mm
            stringBuilder.Append(" --header-center 我是页眉 ");  //设置居中显示页眉
            stringBuilder.Append(" --header-line ");         //页眉和内容之间显示一条直线
            stringBuilder.Append(" --footer-center \"Page [page] of [topage]\" ");    //设置居中显示页脚
            stringBuilder.Append(" --footer-line ");       //页脚和内容之间显示一条直线
            stringBuilder.Append(" " + htmlPath + " ");       //本地 HTML 的文件路径或网页 HTML 的URL地址
            stringBuilder.Append(" " + savePath + " ");       //生成的 PDF 文档的保存路径
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 验证保存路径
        /// </summary>
        /// <param name="savePath"></param>
        private void CheckFilePath(string savePath)
        {
            string ext = string.Empty;
            string path = string.Empty;
            string fileName = string.Empty;

            ext = Path.GetExtension(savePath);
            if (string.IsNullOrEmpty(ext) || ext.ToLower() != ".pdf")
            {
                throw new Exception("Extension error:This method is used to generate PDF files.");
            }

            fileName = Path.GetFileName(savePath);
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("File name is empty.");
            }

            try
            {
                path = savePath.Substring(0, savePath.IndexOf(fileName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {
                throw new Exception("The file path does not exist.");
            }
        }

        /// <summary>
        /// HTML文本内容转HTML文件
        /// </summary>
        /// <param name="strHtml">HTML文本内容</param>
        /// <returns>HTML文件的路径</returns>
        public string HtmlTextConvertFile(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml))
            {
                throw new Exception("HTML text content cannot be empty.");
            }

            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"html\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = path + DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(1000, 10000) + ".html";
                FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
                streamWriter.Write(strHtml);
                streamWriter.Flush();

                streamWriter.Close();
                streamWriter.Dispose();
                fileStream.Close();
                fileStream.Dispose();
                return fileName;
            }
            catch
            {
                throw new Exception("HTML text content error.");
            }
        }
    }
}
