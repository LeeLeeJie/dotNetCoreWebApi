using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///业务基本信息表
    ///</summary>
    [SugarTable("tb_business")]
    public partial class tb_business
    {
           public tb_business(){


           }
           /// <summary>
           /// Desc:业务单号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string business_id {get;set;}

           /// <summary>
           /// Desc:受理时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? input_time {get;set;}

           /// <summary>
           /// Desc:客户名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string customer_name {get;set;}

           /// <summary>
           /// Desc:主办人编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string operator_id {get;set;}

           /// <summary>
           /// Desc:业务类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string business_type {get;set;}

           /// <summary>
           /// Desc:业务获取人编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string original_id {get;set;}

           /// <summary>
           /// Desc:业务所属分公司编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string branch_id {get;set;}

           /// <summary>
           /// Desc:主流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? wf_id {get;set;}

           /// <summary>
           /// Desc:子流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? wf_node_id {get;set;}

           /// <summary>
           /// Desc:业务流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string process_id {get;set;}

           /// <summary>
           /// Desc:客户身份证号码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string identify_card {get;set;}

           /// <summary>
           /// Desc:1为线上费率，0为线下费率
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? business_out_type {get;set;}

           /// <summary>
           /// Desc:最后审批人,用于撤销
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string approver_id {get;set;}

           /// <summary>
           /// Desc:线下出款则为null，还款方式ID，1到期还本息，2每月付息到期还本，5每月等额本息
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? repayment_type_id {get;set;}

           /// <summary>
           /// Desc:借款金额
           /// Default:0.00
           /// Nullable:False
           /// </summary>           
           public decimal borrow_money {get;set;}

           /// <summary>
           /// Desc:借款期限
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? borrow_limit {get;set;}

           /// <summary>
           /// Desc:年化利率
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? loan_rate {get;set;}

           /// <summary>
           /// Desc:流程所属分公司编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string flow_branch_id {get;set;}

           /// <summary>
           /// Desc:业务部门
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string business_dept {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string flow_business_type {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string back_process_id {get;set;}

           /// <summary>
           /// Desc:已编辑过的节点
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string edit_procss_id {get;set;}

           /// <summary>
           /// Desc:客户类型(0和1为个人借款, 2为企业借款)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? customer_type {get;set;}

           /// <summary>
           /// Desc:贷后业务流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string after_load_process_id {get;set;}

           /// <summary>
           /// Desc:贷后主流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? after_wf_id {get;set;}

           /// <summary>
           /// Desc:贷后子流程编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? after_wf_node_id {get;set;}

           /// <summary>
           /// Desc:[具体业务类型分类]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string business_type_detail {get;set;}

           /// <summary>
           /// Desc:发标类型0:线下放款, 1:P2p,2:定期理财
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? issue_type {get;set;}

           /// <summary>
           /// Desc:所有客户姓名,已逗号隔开
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string customer_list {get;set;}

           /// <summary>
           /// Desc:业务来源：0-常规录入 1-结清续贷新业务 2-结清续贷续贷业务 3-线下历史导入 4-扫码业务 5-优质车抵贷
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public byte source_type {get;set;}

           /// <summary>
           /// Desc:原始来源业务的业务编号（目前用于结清续贷业务）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string source_business_id {get;set;}

           /// <summary>
           /// Desc:标识是否P2P拆标业务，0：非P2P拆标业务，1：P2P拆标业务
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? issue_split_type {get;set;}

           /// <summary>
           /// Desc:1是历史数据，其他不是
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? is_old {get;set;}

           /// <summary>
           /// Desc:[是否需要进行平台还款，1：是，0：否]
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? is_tuandai_repay {get;set;}

           /// <summary>
           /// Desc:[是否满足申请房贷存量条件，0  null : 不满足, 1：可以存量，2：已发起存量，3:发起存量并且风控操作保存节点]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? is_can_cunliang {get;set;}

           /// <summary>
           /// Desc:0:新增客户,1:结清再贷客户
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? customer_class {get;set;}

           /// <summary>
           /// Desc:[客户分类明细:1垫资结清]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? customer_detail_class {get;set;}

           /// <summary>
           /// Desc:[平台编号 1：团贷网 2：你我金融, 3:粤财小贷]
           /// Default:1
           /// Nullable:True
           /// </summary>           
           public byte? platform_id {get;set;}

           /// <summary>
           /// Desc:[是否多平台借款，1：是，0：否]
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public byte? multi_platform {get;set;}

           /// <summary>
           /// Desc:还款所在的系统，1表示需要在信贷还款，2表示需要在贷后还款
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? repayment_system {get;set;}

           /// <summary>
           /// Desc:逾期费方案（0、旧方案 1、新方案）
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? late_fee_scheme {get;set;}

           /// <summary>
           /// Desc:韬毓资产 1:是 其他:不是
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public byte? taoyu_assets {get;set;}

    }
}
