using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Wunion.DataAdapter.Kernel.DbInterop
{
    /// <summary>
    /// 用于执行事务释放的函数的委托。
    /// </summary>
    /// <param name="Owner">事务所属的 DBTransactionController 控制器。</param>
    public delegate void ReleaseTransHandler(DBTransactionController Owner);

    /// <summary>
    /// 事务控制器。
    /// </summary>
    public class DBTransactionController : IDisposable
    {
        private string _UniqueId;
        private IDbTransaction TransOper;
        private TransactionDbAccess _DBA;
        private bool _IsCommit;
        private ReleaseTransHandler ReleaseTrans;

        /// <summary>
        /// 创建一个 <see cref="Wunion.DataAdapter.Kernel.DbInterop.DBTransactionController"/> 事务控制器实例。
        /// </summary>
        /// <param name="trans">与数据库相关的事务对象。</param>
        /// <param name="transDBA">事务数据访问器（用于执行受事务控制的命令）。</param>
        /// <param name="release">释放事务的函数。</param>
        internal DBTransactionController(IDbTransaction trans, TransactionDbAccess transDBA, ReleaseTransHandler release)
        {
            TransOper = trans;
            _DBA = transDBA;
            ReleaseTrans = release;
            _UniqueId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取该事务控制器的唯一标识。
        /// </summary>
        public string UniqueId
        {
            get { return _UniqueId; }
        }

        /// <summary>
        /// 获取一个值，该值指示事务是否已提交。
        /// </summary>
        public bool IsCommit
        {
            get { return _IsCommit; }
        }

        /// <summary>
        /// 获取在事务中执行命令的访问器对象。
        /// </summary>
        public TransactionDbAccess DBA
        {
            get { return _DBA; }
        }

        /// <summary>
        /// 提交事务。
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            if (IsCommit)
                return false;
            if (TransOper == null)
                throw new Exception("在提交事务时产生错误：事务控制器为空或已释放。");
            TransOper.Commit();
            _IsCommit = true;
            return true;
        }

        /// <summary>
        /// 回滚事务。
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            if (IsCommit)
                return false;
            if (TransOper == null)
                throw new Exception("在回滚事务时产生错误：事务控制器为空或已释放。");
            TransOper.Rollback();
            _IsCommit = true;
            if (ReleaseTrans != null)
                ReleaseTrans(this);
            return true;
        }

        /// <summary>
        /// 释放该对象所占用的所有资源。
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (TransOper != null)
                {
                    Rollback();
                    TransOper.Dispose();
                    TransOper = null;
                }
                if (ReleaseTrans != null)
                    ReleaseTrans(this);
                _DBA = null;
            }
        }

        /// <summary>
        /// 释放该对象所占用的所有资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 对象终结器。
        /// </summary>
        ~DBTransactionController()
        {
            Dispose(false);
        }
    }
}
