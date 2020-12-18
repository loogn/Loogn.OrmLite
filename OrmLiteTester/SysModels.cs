using System;
using System.ComponentModel;
using Loogn.OrmLite;

namespace OrmLiteTester
{
    public class SysUser
    {
        [DisplayName("编号")]
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [DisplayName("账号")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 1-男，2-女
        /// </summary>
        [DisplayName("性别")]
        public EnumGender? Gender { get; set; }

        /// <summary>
        /// 1-正常,2-禁用
        /// </summary>
        [DisplayName("状态")]
        public EnumStatus Status { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DisplayName("头像")]
        public string Avatar { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [DisplayName("添加时间")]
        public DateTime AddTime { get; set; }

        //public string GetGender()
        //{
        //    if (Gender == 1)
        //    {
        //        return "男";
        //    }
        //    else if (Gender == 2)
        //    {
        //        return "女";
        //    }
        //    else
        //    {
        //        return "未知";
        //    }
        //}

        //public string GetStatus()
        //{
        //    if (Status == 1)
        //    {
        //        return "正常";
        //    }
        //    else if (Status == 2)
        //    {
        //        return "禁用";
        //    }
        //    else
        //    {
        //        return "未知";
        //    }
        //}
    }

    public class SysRes 
    {
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资源地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 连接目标
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 资源类型，1-菜单，2-接口
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 1启用，2禁用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        /// 父级菜单编号
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }


        public string GetStatus()
        {
            return Status switch
            {
                1 => "启用",
                2 => "禁用",
                _ => Type.ToString()
            };
        }

        public new string GetType()
        {
            return Type switch
            {
                1 => "菜单",
                2 => "接口",
                _ => Type.ToString()
            };
        }
    }

    public class SysRole
    {
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
    }

    public class SysRole_Res
    {
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        public long SysRoleId { get; set; }

        public long SysResId { get; set; }
    }

    public class SysUser_Res
    {
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        /// <summary>
        /// 系统用户编号
        /// </summary>
        public long SysUserId { get; set; }

        /// <summary>
        /// 系统资源编号
        /// </summary>
        public long SysResId { get; set; }
    }

    public class SysUser_Role
    {
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public long Id { get; set; }

        public long SysUserId { get; set; }

        public long SysRoleId { get; set; }
    }
}