﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20110414
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20110414
 * 
*******************************************************/
using System;
using System.Linq;
using OEA;
using OEA.MetaModel;
using OEA.MetaModel.Attributes;
using OEA.MetaModel.View;
using OEA.MetaModel.Audit;
using OEA.ManagedProperty;

namespace OEA.Library.Audit
{
    [RootEntity, Serializable]
    [ConditionQueryType(typeof(AuditItemConditionCriteria))]
    public partial class AuditItem : Entity
    {
        public static readonly Property<string> TitleProperty = P<AuditItem>.Register(e => e.Title);
        public string Title
        {
            get { return this.GetProperty(TitleProperty); }
            set { this.SetProperty(TitleProperty, value); }
        }

        public static readonly Property<string> ContentProperty = P<AuditItem>.Register(e => e.Content);
        public string Content
        {
            get { return this.GetProperty(ContentProperty); }
            set { this.SetProperty(ContentProperty, value); }
        }

        public static readonly Property<string> PrivateContentProperty = P<AuditItem>.Register(e => e.PrivateContent);
        public string PrivateContent
        {
            get { return this.GetProperty(PrivateContentProperty); }
            set { this.SetProperty(PrivateContentProperty, value); }
        }

        public static readonly Property<string> UserProperty = P<AuditItem>.Register(e => e.User);
        public string User
        {
            get { return this.GetProperty(UserProperty); }
            set { this.SetProperty(UserProperty, value); }
        }

        public static readonly Property<string> MachineNameProperty = P<AuditItem>.Register(e => e.MachineName);
        public string MachineName
        {
            get { return this.GetProperty(MachineNameProperty); }
            set { this.SetProperty(MachineNameProperty, value); }
        }

        public static readonly Property<string> ModuleNameProperty = P<AuditItem>.Register(a => a.ModuleName);
        public string ModuleName
        {
            get { return this.GetProperty(ModuleNameProperty); }
            set { this.SetProperty(ModuleNameProperty, value); }
        }

        public static readonly Property<AuditLogType> TypeProperty = P<AuditItem>.Register(a => a.Type, AuditLogType.None);
        public AuditLogType Type
        {
            get { return this.GetProperty(TypeProperty); }
            set { this.SetProperty(TypeProperty, value); }
        }

        public static readonly Property<DateTime> LogTimeProperty = P<AuditItem>.Register(a => a.LogTime);
        public DateTime LogTime
        {
            get { return this.GetProperty(LogTimeProperty); }
            set { this.SetProperty(LogTimeProperty, value); }
        }

        /// <summary>
        /// 当前操作的实体对象的ID
        /// </summary>
        public static readonly Property<int?> EntityIdProperty = P<AuditItem>.Register(a => a.EntityId);
        public int? EntityId
        {
            get { return this.GetProperty(EntityIdProperty); }
            set { this.SetProperty(EntityIdProperty, value); }
        }
    }

    [Serializable]
    public partial class AuditItemList : EntityList
    {
        protected override void OnGetAll()
        {
            this.QueryDb(q => q.Order(AuditItem.LogTimeProperty, false));
        }

        /// <summary>
        /// 条件面板查询
        /// </summary>
        /// <param name="criteria"></param>
        protected void QueryBy(AuditItemConditionCriteria criteria)
        {
            this.QueryDb(q =>
            {
                var startDate = criteria.StartTime.Date;
                var endDate = criteria.EndTime.AddDays(1d).Date;

                //拼装查询条件
                q.Constrain(AuditItem.TitleProperty).Like(criteria.TitleKeyWords)
                    .And().Constrain(AuditItem.ContentProperty).Like(criteria.ContentKeyWords)
                    .And().Constrain(AuditItem.UserProperty).Like(criteria.UserKeyWords)
                    .And().Constrain(AuditItem.MachineNameProperty).Like(criteria.MachineKeyWords)
                    //起始时间和终止时间只精确到日。终止时间往后推一天
                    .And().Constrain(AuditItem.LogTimeProperty).GreaterEqual(startDate)
                    .And().Constrain(AuditItem.LogTimeProperty).LessEqual(endDate);

                if (!string.IsNullOrEmpty(criteria.ModuleName))
                {
                    q.And().Constrain(AuditItem.ModuleNameProperty).Equal(criteria.ModuleName);
                }
                if (criteria.AuditLogType != AuditLogType.None)
                {
                    q.And().Constrain(AuditItem.TypeProperty).Equal(criteria.AuditLogType);
                }

                q.Order(AuditItem.LogTimeProperty, false);
            });
        }
    }

    public class AuditItemRepository : EntityRepository
    {
        protected AuditItemRepository() { }
    }

    internal class AuditItemConfig : EntityConfig<AuditItem>
    {
        protected override void ConfigMeta()
        {
            base.ConfigMeta();

            Meta.MapTable().HasColumns(
                AuditItem.TitleProperty,
                AuditItem.ContentProperty,
                AuditItem.PrivateContentProperty,
                AuditItem.UserProperty,
                AuditItem.MachineNameProperty,
                AuditItem.ModuleNameProperty,
                AuditItem.TypeProperty,
                AuditItem.LogTimeProperty,
                AuditItem.EntityIdProperty
                );
        }

        protected override void ConfigView()
        {
            base.ConfigView();

            View.HasLabel("日志").NotAllowEdit();

            if (!OEAEnvironment.IsWeb)
            {
                View.ClearWPFCommands(false)
                    .UseWPFCommands(
                    "RBAC.LoginLogStatisticCommand",
                    "RBAC.ClearAuditLogCommand"
                    );
            }
            else
            {
                View.RemoveWebCommands(WebCommandNames.Add);
            }

            View.Property(AuditItem.TitleProperty).ShowIn(ShowInWhere.All).HasLabel("标题");
            View.Property(AuditItem.ContentProperty).ShowIn(ShowInWhere.All).HasLabel("内容");
            View.Property(AuditItem.UserProperty).ShowIn(ShowInWhere.All).HasLabel("用户");
            View.Property(AuditItem.MachineNameProperty).ShowIn(ShowInWhere.All).HasLabel("机器名");
            View.Property(AuditItem.ModuleNameProperty).ShowIn(ShowInWhere.List).HasLabel("模块名");
            View.Property(AuditItem.TypeProperty).ShowIn(ShowInWhere.List).HasLabel("类型");
            View.Property(AuditItem.LogTimeProperty).ShowIn(ShowInWhere.List).HasLabel("时间");
        }
    }
}