using Autodesk.Revit;
using Autodesk.RevitAddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Reflection;
using System.IO;

namespace RevitFile_compare
{
    public class RevitCoreContext
    {
        // 此路径为动态反射搜索路径 、 此路径可为任意路径（只要路径下有RevitNET 所需依赖项即可，完整依赖项可在 Naviswork 2016 下面找到）

        static readonly string[] Searchs = RevitProductUtility.GetAllInstalledRevitProducts().Select(x => x.InstallLocation).ToArray(); 

        static readonly object lockobj = new object();

        static RevitCoreContext _instance;

        private Product _product;

        public Application Application { get => _product.Application; }

        public static RevitCoreContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockobj)
                    {
                        if (_instance == null)
                        {
                            _instance = new RevitCoreContext();
                        }
                    }
                }

                return _instance;
            }
        }

        static RevitCoreContext()
        {
            AddEnvironmentPaths(Searchs);

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public void Run()
        {
            _product = Product.GetInstalledProduct();

            var clientId = new ClientApplicationId(Guid.NewGuid(), "DotNet", "BIMAPI");

            // I am authorized by Autodesk to use this UI-less functionality. 必须是此字符串。 Autodesk 规定的.

            _product.Init(clientId, "I am authorized by Autodesk to use this UI-less functionality.");
        }

        public void Stop()
        {
            _product?.Exit();
        }

        static void AddEnvironmentPaths(params string[] paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };   /* Searchs.ToList().ForEach(m => System.Windows.MessageBox.Show(m));//有2016也有2018目录路径*/

            var newPath = string.Join(System.IO.Path.PathSeparator.ToString(), path.Concat(paths));     

            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);

            foreach (var item in Searchs)
            {
                var file = string.Format("{0}.dll", System.IO.Path.Combine(item, assemblyName.Name)); /*System.Windows.MessageBox.Show(file);*/

                if (File.Exists(file)||file.Contains("2016"))   //默认2016，若没有2016则以最后一次安装的版本为主
                {
                    return Assembly.LoadFile(file);
                }
                else if (File.Exists(file))
                {
                    return Assembly.LoadFile(file);
                }
            }

            return args.RequestingAssembly;
        }
    }
}
