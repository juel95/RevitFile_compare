using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitFile_compare
{
    public static class DocumentExtension
    {
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "default")
        {
            using (var tr = new Transaction(doc, name))
            {
                tr.Start();

                action(tr);

                var status = tr.GetStatus();

                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        return;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return;
                    default:
                        return;
                }
            }
        }

        public static TResult Invoke<TResult>(this Document doc, Func<Transaction, TResult> func, string name = "default")
        {
            using (var tr = new Transaction(doc, name))
            {
                tr.Start();

                var result = func(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return result;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        return result;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return result;
                    default:
                        return result;
                }
            }
        }

        public static void InvokeSub(this Document doc, Action<SubTransaction> action)
        {
            using (var tr = new SubTransaction(doc))
            {
                tr.Start();

                action(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        break;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return;
                    default:
                        return;
                }
            }
        }


        public static TResult InvokeSub<TResult>(this Document doc, Func<SubTransaction, TResult> func)
        {
            using (var tr = new SubTransaction(doc))
            {
                tr.Start();

                var result = func(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return result;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        return result;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return result;
                    default:
                        return result;
                }
            }
        }

        public static void InvokeGroup(this Document doc, Action<TransactionGroup> action, string name = "default")
        {
            using (var tr = new TransactionGroup(doc, name))
            {
                tr.Start();

                action(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        break;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return;
                    default:
                        return;
                }
            }
        }

        public static TResult InvokeGroup<TResult>(this Document doc, Func<TransactionGroup, TResult> func, string name = "default")
        {
            using (var tr = new TransactionGroup(doc, name))
            {
                tr.Start();

                var result = func(tr);

                var status = tr.GetStatus();
                switch (status)
                {
                    case TransactionStatus.Started:
                        tr.Commit();
                        return result;
                    case TransactionStatus.Committed:
                    case TransactionStatus.RolledBack:
                        return result;
                    case TransactionStatus.Error:
                        tr.RollBack();
                        return result;
                    default:
                        return result;
                }
            }
        }

        public static FilteredElementCollector QueryByType<T>(this Document doc) where T : Element
        {
            return new FilteredElementCollector(doc).OfClass(typeof(T));
        }
    }
}
