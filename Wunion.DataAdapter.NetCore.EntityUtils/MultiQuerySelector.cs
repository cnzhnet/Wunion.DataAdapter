using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Wunion.DataAdapter.Kernel;
using Wunion.DataAdapter.Kernel.CommandBuilders;

namespace Wunion.DataAdapter.EntityUtils
{
    /// <summary>
    /// 表示多表查询筛选器.
    /// </summary>
    public class MultiQuerySelector
    {
        private SelectBlock sBlock;
        private FromBlock fBlock;
        private Dictionary<Type, AgentBufferItem> AgentsIndexes;
        private List<AgentBufferItem> Agents;
        private DataEngine ReadEngine;

        /// <summary>
        /// 创建一个 <see cref="MultiQuerySelector"/> 的对象实例.
        /// </summary>
        /// <param name="engine">用于执行查询的数据引擎.</param>
        internal MultiQuerySelector(DataEngine engine)
        {
            ReadEngine = engine;
            Command = new DbCommandBuilder();
            fBlock = new FromBlock();
            AgentsIndexes = new Dictionary<Type, AgentBufferItem>();
            Agents = new List<AgentBufferItem>();
        }

        /// <summary>
        /// 获取命令.
        /// </summary>
        internal DbCommandBuilder Command { get; set; }

        /// <summary>
        /// 开始建立查询命令对象树.
        /// </summary>
        /// <typeparam name="TEntityAgent">表对应的实体映射代理对象.</typeparam>
        /// <returns></returns>
        internal MultiQuerySelector From<TEntityAgent>() where TEntityAgent : EntityAgent, new()
        {
            sBlock = Command.Select();
            TEntityAgent agent = new TEntityAgent();
            TableMapper context = agent.CreateContext();
            context.SetDataEngine(ReadEngine, null);
            agent.TableContext = context;
            agent.IncludeTableName = true; // 指法查询代理对象在生成字段信息时带上表名.
            // 将实体表的代理类记录下来，后面组织查询时还要用到.
            if (!(AgentsIndexes.ContainsKey(typeof(TEntityAgent))))
            {
                AgentBufferItem BufferItem = new AgentBufferItem(agent);
                AgentsIndexes.Add(typeof(TEntityAgent), BufferItem);
                Agents.Add(BufferItem);
            }
            // 向 FROM 了向中添加查询的表.
            EntityTableAttribute tableAttribute = context.GetTableAttribute();
            fBlock.Content.Add(fm.Table(tableAttribute.TableName));
            sBlock.Blocks.Add(fBlock);
            return this;
        }

        /// <summary>
        /// 向多表查询中添加一个联合查询的表.
        /// </summary>
        /// <typeparam name="TEntityAgent">表的实体映射代理对象.</typeparam>
        /// <returns></returns>
        public EntityJoinSelector Join<TEntityAgent>() where TEntityAgent : EntityAgent, new()
        {
            if (sBlock == null)
                throw new NullReferenceException("Please invoke in advance the \"From\" method.");
            TEntityAgent agent = new TEntityAgent();
            TableMapper context = agent.CreateContext();
            context.SetDataEngine(ReadEngine, null);
            agent.TableContext = context;
            agent.IncludeTableName = true;  // 指法查询代理对象在生成字段信息时带上表名.
            // 将实体表的代理类记录下来，后面组织查询时还要用到.
            if (!(AgentsIndexes.ContainsKey(typeof(TEntityAgent))))
            {
                AgentBufferItem BufferItem = new AgentBufferItem(agent);
                AgentsIndexes.Add(typeof(TEntityAgent), BufferItem);
                Agents.Add(BufferItem);
            }
            // 向 FROM 子向中添加 LEFT JOIN 子句.
            EntityTableAttribute tableAttribute = context.GetTableAttribute();
            LeftJoinDescription leftJoin = fm.LeftJoin(fm.Table(tableAttribute.TableName));
            fBlock.Content.Add(leftJoin);
            // 返回 JEFT JOIN 子句的筛选器构建对象.
            return new EntityJoinSelector(this, leftJoin, (t) => {
                if (AgentsIndexes.ContainsKey(t))
                    return AgentsIndexes[t].Agent<EntityAgent>();
                return null;
            });
        }

        /// <summary>
        /// 构建多表查询条件.
        /// </summary>
        /// <param name="func">用于返回查询条件.</param>
        /// <returns></returns>
        public MultiQuerySelector Where(Func<AgentBufferItem[], object[]> func)
        {
            sBlock.Where(func(Agents.ToArray()));
            return this;
        }

        /// <summary>
        /// 在多表查询中增加 GROUP BY 分组子句.
        /// </summary>
        /// <param name="func">返回用于分组的字段.</param>
        /// <returns></returns>
        public MultiQuerySelector GroupBy(Func<AgentBufferItem[], object[]> func)
        {
            List<FieldDescription> Fields = new List<FieldDescription>();
            FieldDescription field;
            foreach (object item in func(Agents.ToArray()))
            {
                field = item as FieldDescription;
                if (!(object.ReferenceEquals(field, null)))
                    Fields.Add(field);
            }
            if (Fields.Count > 0)
                sBlock.GroupBy(Fields.ToArray());
            return this;
        }

        /// <summary>
        /// 在多表查询中增加 ORDER BY 排序子句.
        /// </summary>
        /// <param name="func">用于返回排序字段.</param>
        /// <param name="sort">排序方式.</param>
        /// <returns></returns>
        public MultiQuerySelector OrderBy(Func<AgentBufferItem[], FieldDescription> func, OrderByMode sort)
        {
            sBlock.OrderBy(func(Agents.ToArray()), sort);
            return this;
        }

        /// <summary>
        /// 在多表查询中创建分布信息.
        /// </summary>
        /// <param name="pageSize">每页数据条数.</param>
        /// <param name="currentPage">当前页（从第1页开始）.</param>
        /// <returns></returns>
        public MultiQuerySelector Paging(int pageSize, int currentPage)
        {
            sBlock.Paging(pageSize, currentPage);
            return this;
        }


        /// <summary>
        /// 执行多表查询并返回结果数据集合.
        /// </summary>
        /// <param name="func">用于创建并返回要查询的字段信息.</param>
        /// <returns></returns>
        public List<dynamic> Select(Func<AgentBufferItem[], object[]> func)
        {
            // 接收 Lambda 表达式返回的查询字段，并添加到 SELECT 命令中.
            // 使用 object[] 的目的为向外隐藏 IDescription 接口
            object[] fields = func(Agents.ToArray());
            int insIndex = sBlock.Blocks.IndexOf(fBlock);
            IDescription descr;
            foreach (object desObject in fields)
            {
                descr = desObject as IDescription;
                if (descr != null) // 当返回的字段元素非 IDescription 对象时忽略它（否则命令解无法解释）.
                    sBlock.Blocks.Insert(insIndex++, descr); // 此时应在 SELECT 命令中的 FROM 子句前插入字段元素，否则解释出的 SQL 命令将会是错误的.
            }
            IDataReader Rd = ReadEngine.ExecuteReader(Command);
            if (Rd == null)
                throw new Exception(ReadEngine.DBA.Error == null ? "查询时产生了未知的错误." : ReadEngine.DBA.Error.Message);
            List<dynamic> Result = new List<dynamic>();
            DynamicEntity entity;
            int i;
            while (Rd.Read())
            {
                entity = new DynamicEntity();
                for (i = 0; i < Rd.FieldCount; ++i)
                    entity.SetPropertyValue(Rd.GetName(i), Rd.GetValue(i), Rd.GetFieldType(i));
                Result.Add(entity);
            }
            Rd.Close();
            Rd.Dispose();
            return Result;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="func">用于创建并返回要查询的字段信息.</param>
        /// <returns></returns>
        public object QueryScalar(Func<AgentBufferItem[], object[]> func)
        {
            return ReadEngine.ExecuteScalar(Command);
        }

        /// <summary>
        /// 表对应的实体映射代理对象的缓存元素对象.
        /// </summary>
        public sealed class AgentBufferItem
        {
            private EntityAgent _Agent;

            /// <summary>
            /// 创建一个 <see cref="AgentBufferItem"/> 的对象实例.
            /// </summary>
            /// <param name="entityAgent">表对应的实体映射代理对象.</param>
            internal AgentBufferItem(EntityAgent entityAgent)
            {
                _Agent = entityAgent;
            }

            /// <summary>
            /// 获得实体代理对象.
            /// </summary>
            /// <typeparam name="TEntityAgent">代理对象的类型.</typeparam>
            /// <returns></returns>
            public TEntityAgent Agent<TEntityAgent>() where TEntityAgent : EntityAgent
            {
                return (TEntityAgent)_Agent;
            }
        }
    }

    /// <summary>
    /// 用于多表查询时的实体筛选器.
    /// </summary>
    public class EntityJoinSelector
    {
        private MultiQuerySelector _Owner;
        private LeftJoinDescription LeftJion;
        private Func<Type, EntityAgent> GetTargetAgent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="leftJion"></param>
        /// <param name="getAgent"></param>
        internal EntityJoinSelector(MultiQuerySelector owner, LeftJoinDescription leftJion, Func<Type, EntityAgent> getAgent)
        {
            _Owner = owner;
            LeftJion = leftJion;
            GetTargetAgent = getAgent;
        }

        /// <summary>
        /// 多表查询的 ON 子句.
        /// </summary>
        /// <typeparam name="TAgent1">左表的实体代理对象.</typeparam>
        /// <typeparam name="TAgent2">右表的实体代理对象.</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public MultiQuerySelector On<TAgent1, TAgent2>(Func<TAgent1, TAgent2, object[]> func) where TAgent1 : EntityAgent where TAgent2 : EntityAgent
        {
            if (_Owner == null)
                throw new NullReferenceException(nameof(_Owner));
            if (LeftJion == null)
                throw new NullReferenceException(nameof(LeftJion));
            if (GetTargetAgent == null)
                throw new NullReferenceException(nameof(GetTargetAgent));

            TAgent1 t1 = (TAgent1)GetTargetAgent(typeof(TAgent1));
            TAgent2 t2 = (TAgent2)GetTargetAgent(typeof(TAgent2));
            LeftJion.ON(func(t1, t2));
            return _Owner;
        }
    }
}
