using CommonTool.DBHelper;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CommonTool
{
    /// <summary>
    /// 权限控制帮助类
    /// </summary>
    public class AuthorityHelper
    {
        public AuthorityHelper()
        {

        }

        /// <summary>
        /// 获取按钮权限
        /// </summary>
        /// <param name="authorityType"></param>
        /// <param name="pageUrl"></param>
        /// <param name="buttonCode"></param>
        /// <returns></returns>
        public static bool GetButtonAuthority(int authorityType, string pageUrl, string buttonCode)
        {

            if (CookieHelper.GetCookie("systemUserAccount", "") == "hrAdmin")
            {
                return true;
            }

            List<Authority> authorityList = DapperHelper.CreateInstance().SimpleQuery<Authority>(@"select * from Authority where AuthorityType='" + authorityType + "' and PageUrl='" + pageUrl + "' and ButtonCode='" + buttonCode + "'");

            string ObjectId = CookieHelper.GetCookie("systemUserObjectId", "");
         
            List<RoleRelationship> relationList = DapperHelper.CreateInstance().SimpleQuery<RoleRelationship>(@"select * from RoleRelationship where UserObjectId='" + ObjectId + "'");

            //如果是HR角色则赋予最高权限
            if (relationList.Where(c => c.RoleName == "HR管理员").ToList().Count() > 0)
            {
                return true;
            }
            if (authorityList.Count > 0)
            {

                string[] RoleObjectIds = authorityList[0].RoleObjectIds.Split(',');
                if (relationList.Count > 0)
                {
                    foreach (var item in relationList)
                    {
                        if (authorityList[0].RoleObjectIds.IndexOf(item.RoleObjectId) > -1)
                        {
                            return authorityList[0].HasAuth;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="authorityType"></param>
        /// <param name="pageUrl"></param>
        /// <param name="buttonCode"></param>
        /// <returns></returns>
        public static bool GetPageAuthority(int authorityType, string pageUrl)
        {

            if (CookieHelper.GetCookie("systemUserAccount", "") == "hrAdmin")
            {
                return true;
            }

            List<Authority> authorityList = DapperHelper.CreateInstance().SimpleQuery<Authority>(@"select * from Authority where AuthorityType='" + authorityType + "' and PageUrl='" + pageUrl + "'");

            string ObjectId = CookieHelper.GetCookie("systemUserObjectId", "");
            List<RoleRelationship> relationList = DapperHelper.CreateInstance().SimpleQuery<RoleRelationship>(@"select * from RoleRelationship where UserObjectId='" + ObjectId + "'");

            //如果是HR角色则赋予最高权限
            if (relationList.Where(c => c.RoleName == "HR管理员").ToList().Count() > 0)
            {
                return true;
            }

            if (authorityList.Count > 0)
            {
            
                string[] RoleObjectIds = authorityList[0].RoleObjectIds.Split(',');
                if (relationList.Count > 0)
                {
                    foreach (var item in relationList)
                    {
                        foreach (var authItem in authorityList)
                        {
                            if (authItem.RoleObjectIds.IndexOf(item.RoleObjectId) > -1)
                            {
                                return authItem.HasAuth;
                            }

                        }

                    }
                }
                else
                {
                    return false;
                }


            }
            return false;

        }
    }
}
