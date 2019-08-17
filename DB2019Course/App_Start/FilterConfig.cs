using DB2019Course.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace DB2019Course
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class MyAuthorizeAttribute : AuthorizeAttribute, IAuthenticationFilter
    {

        private Entities db = new Entities(); //экземпляр базы данных

        public void OnAuthentication(AuthenticationContext filterContext)
        {
          //требуется для интерфейса, реально вся работа в AuthorizeCore   
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = db.Logged.Find(filterContext.HttpContext.Session.SessionID); //Если есть такой юзер
            if (filterContext.Result == null) //если не залогинен
            if (user == null) //и нет такой сессии
            {
                filterContext.Result = new RedirectToRouteResult( //отправляем логиниться
                    new RouteValueDictionary {
                        { "controller", "Auths" }, { "action", "Login" }
                   });
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult( //иначе, значит, нет доступа роли
                    new RouteValueDictionary {
                    { "controller", "Auths" }, { "action", "NotAuthorized" }}
                    );
            }
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var user = db.Logged.Find(filterContext.HttpContext.Session.SessionID); //то же самое, что и
            if (user == null)                                                       //в предыдущем методе
            {
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Auths", action = "Login" })
                    );
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Auths", action = "NotAuthorized" })
                );
            }
        }

        public bool IsInRole(string roles, string name)
        {
            return roles is null || roles == "" || (roles.IndexOf(db.Auth.Find(name).Role)>-1);
            //ролей нет или в строке ролей есть  роль текущего пользователя
        }
        
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var logged = db.Logged.Find(httpContext.Session.SessionID); //ищем залогиненного
            if (db.Logged.Find(httpContext.Session.SessionID)!=null && IsInRole(Roles, logged.Auth.Login)) //если залогинен 
            {                                                                                              //и нужная роль
                return true; //пускаем
            }
            return false; //иначе нет
        }
    }

}
