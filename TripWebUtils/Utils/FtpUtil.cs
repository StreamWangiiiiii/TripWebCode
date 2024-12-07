using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace TripWebUtils.Utils;

#region "文件信息结构"

    public struct FileStruct
    {
        public int Size;
        public string Flags;
        public string Owner;
        public string Group;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
    }
    public enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }
    #endregion

    public class FtpUtil
    {

        //Friend client As New MyWebClient

        #region "属性信息"

        /// FTP请求对象
        private FtpWebRequest Request = null;
        /// FTP响应对象
        private FtpWebResponse Response = null;
        /// FTP服务器地址
        private Uri _Uri;
        /// FTP服务器地址
        public Uri Uri
        {
            get
            {
                if (_DirectoryPath == "/")
                {
                    return _Uri;
                }
                else
                {
                    string strUri = _Uri.ToString();
                    if (strUri.EndsWith("/"))
                    {
                        strUri = strUri.Substring(0, strUri.Length - 1);
                    }
                    return new Uri(strUri + this.DirectoryPath);
                }
            }
            set
            {
                if (value.Scheme != Uri.UriSchemeFtp)
                {
                    throw new Exception("wrong FTP address");
                }
                _Uri = new Uri(value.GetLeftPart(UriPartial.Authority));
                _DirectoryPath = value.AbsolutePath;
                if (!_DirectoryPath.EndsWith("/"))
                {
                    _DirectoryPath += "/";
                }
            }
        }

        /// 当前工作目录

        private string _DirectoryPath;
        /// 当前工作目录
        public string DirectoryPath
        {
            get { return _DirectoryPath; }
            set { _DirectoryPath = value; }
        }

        /// FTP登录用户

        private string _UserName;
        /// FTP登录用户
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// 错误信息
        private string _ErrorMsg;
        /// 错误信息
        public string ErrorMsg
        {
            get { return _ErrorMsg; }
            set { _ErrorMsg = value; }
        }

        /// FTP登录密码
        private string _Password;
        /// FTP登录密码
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        /// 连接FTP服务器的代理服务
        private WebProxy _Proxy = null;
        /// 连接FTP服务器的代理服务
        public WebProxy Proxy
        {
            get { return _Proxy; }
            set { _Proxy = value; }
        }

        /// 是否需要删除临时文件
        private bool _isDeleteTempFile = false;
        /// 异步上传所临时生成的文件
        #endregion
        private string _UploadTempFile = "";

        /// <summary>
        /// 默认编码格式
        /// </summary>
        private Encoding _Encoding = Encoding.Default;
        #region "事件"

        public delegate void De_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);
        public delegate void De_DownloadDataCompleted(object sender, AsyncCompletedEventArgs e);
        public delegate void De_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e);
        public delegate void De_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e);
        public delegate void De_TransferError(string ErrorType);

        /// 异步下载进度发生改变触发的事件
        /// 
        public event De_DownloadProgressChanged DownloadProgressChanged;
        /// 异步下载文件完成之后触发的事件
        public event De_DownloadDataCompleted DownloadDataCompleted;
        /// 异步上传进度发生改变触发的事件
        public event De_UploadProgressChanged UploadProgressChanged;
        /// 异步上传文件完成之后触发的事件
        public event De_UploadFileCompleted UploadFileCompleted;
        public event De_TransferError TransferError;

        #endregion

        #region "构造析构函数"

        /// 构造函数
        /// <param name="FtpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>

        public FtpUtil(string ftpUri, string strUserName, string strPassword)
        {
            Uri FtpUri = new Uri(ftpUri);
            this._Uri = new Uri(new Uri(ftpUri).GetLeftPart(UriPartial.Authority));
            _DirectoryPath = FtpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = strUserName;
            this._Password = strPassword;
            this._Proxy = null;
        }

        public FtpUtil(Uri FtpUri, string strUserName, string strPassword)
        {
            this._Uri = new Uri(FtpUri.GetLeftPart(UriPartial.Authority));
            _DirectoryPath = FtpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = strUserName;
            this._Password = strPassword;
            this._Proxy = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="FtpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>
        /// <param name="objProxy">连接代理</param>
        public FtpUtil(string ftpUri, string strUserName, string strPassword, WebProxy objProxy)
        {
            Uri FtpUri = new Uri(ftpUri);
            this._Uri = new Uri(FtpUri.GetLeftPart(UriPartial.Authority));
            _DirectoryPath = FtpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = strUserName;
            this._Password = strPassword;
            this._Proxy = objProxy;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="FtpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>
        /// <param name="encoding">编码格式</param>
        public FtpUtil(Uri ftpUri, string strUserName, string strPassword, Encoding encoding)
        {
            this._Uri = new Uri(ftpUri.GetLeftPart(UriPartial.Authority));
            _DirectoryPath = ftpUri.AbsolutePath;
            if (!_DirectoryPath.EndsWith("/"))
            {
                _DirectoryPath += "/";
            }
            this._UserName = strUserName;
            this._Password = strPassword;
            this._Encoding = encoding;
            this._Proxy = null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FtpUtil()
        {
            this._UserName = "anonymous";
            //匿名用户
            this._Password = "@anonymous";
            this._Uri = null;
            this._Proxy = null;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FtpUtil()
        {
            if (Response != null)
            {
                Response.Close();
                Response = null;
            }
            if (Request != null)
            {
                Request.Abort();
                Request = null;
            }
        }
        #endregion

        #region "建立连接"

        /// <summary>
        /// 建立FTP链接,返回响应对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="FtpMathod">操作命令</param>

        private FtpWebResponse Open(Uri uri, string FtpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = FtpMathod;
                Request.UseBinary = true;
                Request.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (this.Proxy != null)
                {
                    Request.Proxy = this.Proxy;
                }
                FtpWebResponse a = (FtpWebResponse)Request.GetResponse();
                return a;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("open");
                }
                return null;
                //Throw ep
            }
        }
        /// <summary>
        /// 建立FTP链接,返回请求对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="FtpMathod">操作命令</param>

        private FtpWebRequest OpenRequest(Uri uri, string FtpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = FtpMathod;
                Request.UseBinary = true;
                Request.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (this.Proxy != null)
                {
                    Request.Proxy = this.Proxy;
                }
                return Request;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("open");
                }
                return null;
                //Throw ep
            }
        }
        #endregion

        #region "下载文件"


        /// <summary>
        /// 从FTP服务器下载文件，使用与远程文件同名的文件名来保存文件
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        /// <param name="LocalPath">本地路径</param>


        public bool DownloadFile(string RemoteFileName, string LocalPath)
        {
            return DownloadFile(RemoteFileName, LocalPath, RemoteFileName);
        }
        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        /// <param name="LocalPath">保存文件的本地路径,后面带有"\"</param>
        /// <param name="LocalFileName">保存本地的文件名</param>

        public bool DownloadFile(string RemoteFileName, string LocalPath, string LocalFileName)
        {
            byte[] bt = null;
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(LocalFileName) || !IsValidPathChars(LocalPath))
                {
                    throw new Exception("wrong filename or directory");
                }
                if (!Directory.Exists(LocalPath))
                {
                    throw new Exception("can't found " + LocalPath);
                }

                string LocalFullPath = Path.Combine(LocalPath, LocalFileName);
                //If File.Exists(LocalFullPath) Then
                //    Throw New Exception("found same file")
                //End If
                bt = DownloadFile(RemoteFileName);
                if (bt != null)
                {
                    FileStream stream = new FileStream(LocalFullPath, FileMode.Create);
                    stream.Write(bt, 0, bt.Length);
                    stream.Flush();
                    stream.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("download");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="ftpFath"></param>
        /// <returns></returns>
        public Stream GetStream(string ftpFath)
        {
            Stream stream = null;
            try
            {
                Response = Open(new Uri(ftpFath), WebRequestMethods.Ftp.DownloadFile);
                stream = Response.GetResponseStream();
            }
            catch (Exception ex)
            {
                stream = null;
                throw ex;
            }
            return stream;
        }

        /// <summary>
        /// 从FTP服务器下载文件，返回文件二进制数据
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public byte[] DownloadFile(string RemoteFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename or directory");
                }
                Response = Open(new Uri(this.Uri.ToString() + RemoteFileName), WebRequestMethods.Ftp.DownloadFile);
                Stream Reader = Response.GetResponseStream();

                MemoryStream mem = new MemoryStream(1024 * 500);
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int TotalByteRead = 0;
                while (true)
                {
                    bytesRead = Reader.Read(buffer, 0, buffer.Length);
                    TotalByteRead += bytesRead;
                    if (bytesRead == 0)
                    {
                        break; // TODO: might not be correct. Was : Exit While
                    }
                    mem.Write(buffer, 0, bytesRead);
                }
                if (mem.Length > 0)
                {
                    return mem.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("download");
                }
                return null;
                //Throw ep
            }
        }
        #endregion

        #region "异步下载文件"

        /// <summary>
        /// 从FTP服务器异步下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        /// <param name="LocalPath">保存文件的本地路径,后面带有"\"</param>
        /// <param name="LocalFileName">保存本地的文件名</param>

        public void DownloadFileAsync(string RemoteFileName, string LocalPath, string LocalFileName)
        {
            RemoteFileName = RemoteFileName.Replace("\\", "/").Replace("//", "/");
            //byte[] bt = null;
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(LocalFileName) || !IsValidPathChars(LocalPath))
                {
                    throw new Exception("wrong filename or directory");
                }
                if (!Directory.Exists(LocalPath))
                {
                    throw new Exception("can't found " + LocalPath);
                }

                string LocalFullPath = Path.Combine(LocalPath, LocalFileName);
                //If File.Exists(LocalFullPath) Then
                //    'Throw New Exception("found same file")
                //    Throw New Exception("found same file")
                //End If

                DownloadFileAsync(RemoteFileName, LocalFullPath);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("download");
                }
                //Throw ep
            }
        }

        /// <summary>
        /// 从FTP服务器异步下载文件，指定本地完整路径文件名
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        /// <param name="LocalFullPath">本地完整路径文件名</param>

        public void DownloadFileAsync(string RemoteFileName, string LocalFullPath)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename or directory");
                }
                //If File.Exists(LocalFullPath) Then
                //    Throw New Exception("found same file")
                //End If
                MyWebClient client = new MyWebClient();
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
                client.Credentials = new NetworkCredential(this.UserName, this.Password);
                if (this.Proxy != null)
                {
                    client.Proxy = this.Proxy;
                }
                try
                {
                    FileStream fs = File.OpenWrite(LocalFullPath);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    ErrorMsg = "文件正在使用" + ex.Message;
                    if (TransferError != null)
                    {
                        TransferError("download");
                    }
                }
                //Try
                //    File.Move(LocalFullPath, LocalFullPath & ".tmp")
                //Catch ex As Exception
                //    ErrorMsg = ex.Message
                //    RaiseEvent TransferError("download")
                //End Try
                client.DownloadFileAsync(new Uri(this.Uri.ToString() + RemoteFileName), LocalFullPath);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("download");
                }
                //Throw ep
            }
        }

        /// <summary>
        /// 异步下载文件完成之后触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">数据信息对象</param>

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DownloadDataCompleted != null)
            {
                DownloadDataCompleted(sender, e);
            }
        }

        /// <summary>
        /// 异步下载进度发生改变触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">进度信息对象</param>

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(sender, e);
            }
        }
        #endregion

        #region "上传文件"

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件名</param>

        public bool UploadFile(string LocalFullPath)
        {
            return UploadFile(LocalFullPath, Path.GetFileName(LocalFullPath), false);
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public bool UploadFile(string LocalFullPath, bool OverWriteRemoteFile)
        {
            return UploadFile(LocalFullPath, Path.GetFileName(LocalFullPath), OverWriteRemoteFile);
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>

        public bool UploadFile(string LocalFullPath, string RemoteFileName)
        {
            return UploadFile(LocalFullPath, RemoteFileName, false);
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件名</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public bool UploadFile(string LocalFullPath, string RemoteFileName, bool OverWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(Path.GetFileName(LocalFullPath)) || !IsValidPathChars(Path.GetDirectoryName(LocalFullPath)))
                {
                    throw new Exception("wrong filename or directory");
                }
                if (File.Exists(LocalFullPath))
                {
                    FileStream Stream = new FileStream(LocalFullPath, FileMode.Open, FileAccess.Read);
                    byte[] bt = new byte[Stream.Length];
                    Stream.Read(bt, 0, Convert.ToInt32(Stream.Length));
                    //注意，因为Int32的最大限制，最大上传文件只能是大约2G多一点
                    Stream.Close();
                    return UploadFile(bt, RemoteFileName, OverWriteRemoteFile);
                }
                else
                {
                    throw new Exception("can't fount " + LocalFullPath);
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("upload");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="FileBytes">上传的二进制数据</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>

        public bool UploadFile(byte[] FileBytes, string RemoteFileName)
        {
            if (!IsValidFileChars(RemoteFileName))
            {
                throw new Exception("wrong filename or directory");
            }
            return UploadFile(FileBytes, RemoteFileName, false);
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="FileBytes">文件二进制内容</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public bool UploadFile(byte[] FileBytes, string RemoteFileName, bool OverWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename");
                }
                if (!OverWriteRemoteFile && FileExist(RemoteFileName))
                {
                    throw new Exception("found same file on FTP");
                }
                Response = Open(new Uri(this.Uri.ToString() + RemoteFileName), WebRequestMethods.Ftp.UploadFile);
                Stream requestStream = Request.GetRequestStream();
                MemoryStream mem = new MemoryStream(FileBytes);

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int TotalRead = 0;
                while (true)
                {
                    bytesRead = mem.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break; // TODO: might not be correct. Was : Exit While
                    }
                    TotalRead += bytesRead;
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Close();
                Response = (FtpWebResponse)Request.GetResponse();
                mem.Close();
                mem.Dispose();
                FileBytes = null;
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("upload");
                }
                return false;
                //Throw ep
            }
        }
        #endregion

        #region "异步上传文件"

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件名</param>

        public void UploadFileAsync(string LocalFullPath)
        {
            UploadFileAsync(LocalFullPath, Path.GetFileName(LocalFullPath), false);
        }

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public void UploadFileAsync(string LocalFullPath, bool OverWriteRemoteFile)
        {
            UploadFileAsync(LocalFullPath, Path.GetFileName(LocalFullPath), OverWriteRemoteFile);
        }

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>

        public void UploadFileAsync(string LocalFullPath, string RemoteFileName)
        {
            UploadFileAsync(LocalFullPath, RemoteFileName, false);
        }

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="LocalFullPath">本地带有完整路径的文件名</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public void UploadFileAsync(string LocalFullPath, string RemoteFileName, bool OverWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(Path.GetFileName(LocalFullPath)) || !IsValidPathChars(Path.GetDirectoryName(LocalFullPath)))
                {
                    throw new Exception("wrong filename or directory");
                }
                if (!OverWriteRemoteFile && FileExist(RemoteFileName))
                {
                    throw new Exception("found same file on FTP");
                }
                if (File.Exists(LocalFullPath))
                {
                    MyWebClient client = new MyWebClient();

                    client.UploadProgressChanged += client_UploadProgressChanged;
                    client.UploadFileCompleted += client_UploadFileCompleted;
                    client.Credentials = new NetworkCredential(this.UserName, this.Password);
                    if (this.Proxy != null)
                    {
                        client.Proxy = this.Proxy;
                    }

                    client.UploadFileAsync(new Uri(this.Uri.ToString() + RemoteFileName), LocalFullPath);
                }
                else
                {
                    throw new Exception("can't found " + LocalFullPath);
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("upload");
                }
                //Throw ep
            }
        }

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="FileBytes">上传的二进制数据</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>

        public void UploadFileAsync(byte[] FileBytes, string RemoteFileName)
        {
            if (!IsValidFileChars(RemoteFileName))
            {
                throw new Exception("wrong filename or directory");
            }
            UploadFileAsync(FileBytes, RemoteFileName, false);
        }

        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="FileBytes">文件二进制内容</param>
        /// <param name="RemoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="OverWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>

        public void UploadFileAsync(byte[] FileBytes, string RemoteFileName, bool OverWriteRemoteFile)
        {

            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename");
                }
                if (!OverWriteRemoteFile && FileExist(RemoteFileName))
                {
                    throw new Exception("found same file on FTP");
                }
                string TempPath = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                if (!TempPath.EndsWith("\\"))
                {
                    TempPath += "\\";
                }
                string TempFile = TempPath + Path.GetRandomFileName();
                TempFile = Path.ChangeExtension(TempFile, Path.GetExtension(RemoteFileName));
                FileStream Stream = new FileStream(TempFile, FileMode.CreateNew, FileAccess.Write);
                Stream.Write(FileBytes, 0, FileBytes.Length);
                //注意，因为Int32的最大限制，最大上传文件只能是大约2G多一点
                Stream.Flush();
                Stream.Close();
                Stream.Dispose();
                _isDeleteTempFile = true;
                _UploadTempFile = TempFile;
                FileBytes = null;
                UploadFileAsync(TempFile, RemoteFileName, OverWriteRemoteFile);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("upload");
                }
                //Throw ep
            }
        }

        /// <summary>
        /// 异步上传文件完成之后触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">数据信息对象</param>

        private void client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (_isDeleteTempFile)
            {
                if (File.Exists(_UploadTempFile))
                {
                    File.SetAttributes(_UploadTempFile, FileAttributes.Normal);
                    File.Delete(_UploadTempFile);
                }
                _isDeleteTempFile = false;
            }
            if (UploadFileCompleted != null)
            {
                UploadFileCompleted(sender, e);
            }
        }

        /// <summary>
        /// 异步上传进度发生改变触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">进度信息对象</param>

        private void client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (UploadProgressChanged != null)
            {
                UploadProgressChanged(sender, e);
            }
        }
        #endregion

        #region "列出目录文件信息"

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件和目录
        /// </summary>

        public FileStruct[] ListFilesAndDirectories()
        {
            try
            {
                Response = Open(this.Uri, WebRequestMethods.Ftp.ListDirectoryDetails);
                if (Response == null)
                {
                    ErrorMsg = "Can't link to FTP";
                    if (TransferError != null)
                    {
                        TransferError("dir");
                    }
                    return null;
                }
                StreamReader stream = new StreamReader(Response.GetResponseStream(), _Encoding);
                string Datastring = stream.ReadToEnd();
                FileStruct[] list = GetList(Datastring);
                return list;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return null;
            }
        }

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件
        /// </summary>

        public FileStruct[] ListFiles()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile.ToArray();
        }

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有的目录
        /// </summary>

        public FileStruct[] ListDirectories()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory.ToArray();
        }

        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>

        private FileStruct[] GetList(string datastring)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split(ControlChars.Lf);
            FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (_directoryListStyle != FileListStyle.Unknown && !string.IsNullOrEmpty(s))
                {
                    FileStruct f = new FileStruct();
                    f.Name = "..";
                    switch (_directoryListStyle)
                    {
                        case FileListStyle.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break; // TODO: might not be correct. Was : Exit Select

                        //break;
                        case FileListStyle.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s);
                            break; // TODO: might not be correct. Was : Exit Select

                            //break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray.ToArray();
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>

        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse((dateStr + " ") + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }


        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="recordList">文件信息列表</param>

        private FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                else if (s.Length > 8 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }

        bool IsNumeric(string value)
        {
            try
            {
                Int64.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string[] tempdata = Record.Split(' ');
            try
            {
                f.Size = int.Parse((IsNumeric(tempdata[tempdata.Length - 5]) ? tempdata[tempdata.Length - 5] : tempdata[tempdata.Length - 6]));
            }
            catch (Exception ex)
            {
                string errs = ex.Message;
                f.Size = -1;
            }

            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)
            {
                //time
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;
            //最后就是名称
            return f;
        }

        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>

        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion

        #region "目录或文件存在的判断"

        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>

        public bool DirectoryExist(string RemoteDirectoryName)
        {
            try
            {
                if (!IsValidPathChars(RemoteDirectoryName))
                {
                    throw new Exception("wrong directory");
                }
                FileStruct[] listDir = ListDirectories();
                foreach (FileStruct dir in listDir)
                {
                    if (dir.Name == RemoteDirectoryName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 判断一个远程文件是否存在服务器当前目录下面
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>

        public bool FileExist(string RemoteFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename");
                }
                FileStruct[] listFile = ListFiles();
                foreach (FileStruct file in listFile)
                {
                    if (file.Name == RemoteFileName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("file");
                }
                return false;
                //Throw ep
            }
        }


        #endregion

        #region "删除文件"

        /// <summary>
        /// 从FTP服务器上面删除一个文件
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>

        public bool DeleteFile(string RemoteFileName)
        {
            try
            {
                RemoteFileName = RemoteFileName.Replace("//", "/");
                if (!IsValidFileChars(RemoteFileName))
                {
                    throw new Exception("wrong filename");
                }
                Response = Open(new Uri(this.Uri.ToString() + RemoteFileName), WebRequestMethods.Ftp.DeleteFile);
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("file");
                }
                return false;
                //Throw ep
            }
        }
        #endregion

        #region "重命名文件"

        /// <summary>
        /// 更改一个文件的名称或一个目录的名称
        /// </summary>
        /// <param name="RemoteFileName">原始文件或目录名称</param>
        /// <param name="NewFileName">新的文件或目录的名称</param>

        public bool ReName(string RemoteFileName, string NewFileName)
        {
            try
            {
                if (!IsValidFileChars(RemoteFileName) || !IsValidFileChars(NewFileName))
                {
                    throw new Exception("wrong filename");
                }
                if (RemoteFileName == NewFileName)
                {
                    return true;
                }
                if (FileExist(RemoteFileName))
                {
                    Request = OpenRequest(new Uri(this.Uri.ToString() + RemoteFileName), WebRequestMethods.Ftp.Rename);
                    Request.RenameTo = NewFileName;

                    Response = (FtpWebResponse)Request.GetResponse();
                }
                else
                {
                    throw new Exception("can't found " + RemoteFileName + " on FTP");
                }
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return false;
                //Throw ep
            }
        }
        #endregion

        #region "拷贝、移动文件"

        /// <summary>
        /// 把当前目录下面的一个文件拷贝到服务器上面另外的目录中，注意，拷贝文件之后，当前工作目录还是文件原来所在的目录
        /// </summary>
        /// <param name="RemoteFile">当前目录下的文件名</param>
        /// <param name="DirectoryName">新目录名称。
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        /// <returns></returns>

        public bool CopyFileToAnotherDirectory(string RemoteFile, string DirectoryName)
        {
            string CurrentWorkDir = this.DirectoryPath;
            try
            {
                byte[] bt = DownloadFile(RemoteFile);
                GotoDirectory(DirectoryName);
                bool Success = UploadFile(bt, RemoteFile, false);
                this.DirectoryPath = CurrentWorkDir;
                return Success;
            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkDir;
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("file");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 把当前目录下面的一个文件移动到服务器上面另外的目录中，注意，移动文件之后，当前工作目录还是文件原来所在的目录
        /// </summary>
        /// <param name="RemoteFile">当前目录下的文件名</param>
        /// <param name="DirectoryName">新目录名称。
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        /// <returns></returns>

        public bool MoveFileToAnotherDirectory(string RemoteFile, string DirectoryName)
        {
            string CurrentWorkDir = this.DirectoryPath;
            try
            {
                if (string.IsNullOrEmpty(DirectoryName))
                {
                    return false;
                }
                if (!DirectoryName.StartsWith("/"))
                {
                    DirectoryName = "/" + DirectoryName;
                }
                if (!DirectoryName.EndsWith("/"))
                {
                    DirectoryName += "/";
                }
                bool Success = ReName(RemoteFile, DirectoryName + RemoteFile);
                this.DirectoryPath = CurrentWorkDir;
                return Success;
            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkDir;
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("file");
                }
                return false;
                //Throw ep
            }
        }
        #endregion

        #region "建立、删除子目录"

        /// <summary>
        /// 在FTP服务器上当前工作目录建立一个子目录
        /// </summary>
        /// <param name="DirectoryName">子目录名称</param>

        public bool MakeDirectory(string DirectoryName)
        {
            try
            {
                if (!IsValidPathChars(DirectoryName))
                {
                    throw new Exception("wrong directory");
                }
                if (DirectoryExist(DirectoryName))
                {
                    throw new Exception("found same filename or directory on FTP");
                }
                Response = Open(new Uri(this.Uri.ToString() + DirectoryName), WebRequestMethods.Ftp.MakeDirectory);
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 从当前工作目录中删除一个子目录
        /// </summary>
        /// <param name="DirectoryName">子目录名称</param>

        public bool RemoveDirectory(string DirectoryName)
        {
            try
            {
                if (!IsValidPathChars(DirectoryName))
                {
                    throw new Exception("wrong directory");
                }
                if (!DirectoryExist(DirectoryName))
                {
                    throw new Exception("can't found " + DirectoryName + " on FTP");
                }
                Response = Open(new Uri(this.Uri.ToString() + DirectoryName), WebRequestMethods.Ftp.RemoveDirectory);
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return false;
                //Throw ep
            }
        }
        #endregion

        #region "文件、目录名称有效性判断"

        /// <summary>
        /// 判断目录名中字符是否合法
        /// </summary>
        /// <param name="DirectoryName">目录名称</param>

        public bool IsValidPathChars(string DirectoryName)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            char[] DirChar = DirectoryName.ToCharArray();
            foreach (char C in DirChar)
            {
                if (Array.BinarySearch(invalidPathChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断文件名中字符是否合法
        /// </summary>
        /// <param name="FileName">文件名称</param>

        public bool IsValidFileChars(string FileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            char[] NameChar = FileName.ToCharArray();
            foreach (char C in NameChar)
            {
                if (Array.BinarySearch(invalidFileChars, C) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region "目录切换操作"

        /// <summary>
        /// 进入一个目录
        /// </summary>
        /// <param name="DirectoryName">
        /// 新目录的名字。
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>

        public bool GotoDirectory(string DirectoryName)
        {
            string CurrentWorkPath = this.DirectoryPath;
            try
            {
                DirectoryName = DirectoryName.Replace("\\", "/");
                string[] DirectoryNames = DirectoryName.Split(new char[] { '/' });
                if (DirectoryNames[0] == ".")
                {
                    this.DirectoryPath = "/";
                    if (DirectoryNames.Length == 1)
                    {
                        return true;
                    }
                    Array.Clear(DirectoryNames, 0, 1);
                }
                bool Success = false;
                foreach (string dir in DirectoryNames)
                {
                    if (dir != null)
                    {
                        Success = EnterOneSubDirectory(dir);
                        if (!Success)
                        {
                            this.DirectoryPath = CurrentWorkPath;
                            return false;
                        }
                    }
                }

                return Success;
            }
            catch (Exception ep)
            {
                this.DirectoryPath = CurrentWorkPath;
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                //Throw ep
            }
            return true;
        }

        /// <summary>
        /// 从当前工作目录进入一个子目录
        /// </summary>
        /// <param name="DirectoryName">子目录名称</param>

        private bool EnterOneSubDirectory(string DirectoryName)
        {
            try
            {
                if (DirectoryName.IndexOf("/") >= 0 || !IsValidPathChars(DirectoryName))
                {
                    throw new Exception("wrong directory");
                }
                if (DirectoryName.Length > 0 && DirectoryExist(DirectoryName))
                {
                    if (!DirectoryName.EndsWith("/"))
                    {
                        DirectoryName += "/";
                    }
                    _DirectoryPath += DirectoryName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.Message;
                if (TransferError != null)
                {
                    TransferError("dir");
                }
                return false;
                //Throw ep
            }
        }

        /// <summary>
        /// 从当前工作目录往上一级目录
        /// </summary>

        public bool ComeoutDirectory()
        {
            if (_DirectoryPath == "/")
            {
                //ErrorMsg = ""
                //Throw New Exception("当前目录已经是根目录！")
                return true;
            }
            char[] sp = new char[1] { '/' };

            string[] strDir = _DirectoryPath.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            if (strDir.Length == 1)
            {
                _DirectoryPath = "/";
            }
            else
            {
                _DirectoryPath = String.Join("/", strDir, 0, strDir.Length - 1);
            }

            return true;
        }
        #endregion

        #region "重载WebClient，支持FTP进度"

        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                FtpWebRequest req = (FtpWebRequest)base.GetWebRequest(address);
                req.UsePassive = false;
                return req;
            }
        }
        #endregion

    }