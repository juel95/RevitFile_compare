using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// MD5加密类
/// </summary>
public class MD5
{
    public static string GetMD5Hash(string str)
    {
        //就是比string往后一直加要好的优化容器
        StringBuilder sb = new StringBuilder();
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            //将输入字符串转换为字节数组并计算哈希。
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            //X为     十六进制 X都是大写 x都为小写
            //2为 每次都是两位数
            //假设有两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A。 
            //遍历哈希数据的每个字节
            //并将每个字符串格式化为十六进制字符串。
            int length = data.Length;
            for (int i = 0; i < length; i++)
                sb.Append(data[i].ToString("X2"));

        }
        return sb.ToString();
    }

    //验证
    public static bool VerifyMD5Hash(string filePath1, string filePath2)
    {
        if (filePath1.CompareTo(filePath2) == 0)
        {
            System.Windows.MessageBox.Show("两个文件路径相同,请重新选择!");
            return true;
        }
        else
        {
            var str1 = GetMD5Hash_file(filePath1);
            var str2 = GetMD5Hash_file(filePath2);
            if (str1.CompareTo(str2) == 0)
            {
                System.Windows.MessageBox.Show("两个文件完全相同！");
                return true;
            }
            else
                return false;
        }
    }
    public static string GetMD5Hash_file(string fileName)
    {
        StringBuilder sb = new StringBuilder();
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                //将输入字符串转换为字节数组并计算哈希。
                byte[] data = md5.ComputeHash(fs);

                //X为     十六进制 X都是大写 x都为小写
                //2为 每次都是两位数
                //假设有两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A。 
                //遍历哈希数据的每个字节
                //并将每个字符串格式化为十六进制字符串。
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(data[i].ToString("X2"));
                fs.Flush();
                fs?.Close();
            }
            catch 
            {

            }
        }
        return sb.ToString();
    }
}
