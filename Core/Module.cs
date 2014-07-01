namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Nancy;

    public class Module : NancyModule
    {
        public Module()
        {
            Get["/hello"] = _ =>
            {
                return Negotiate.WithView("hello");
            };
            Get["/time"] = _ =>
            {
                return Negotiate.WithView("time");
            };
            Get["/image"] = _ =>
            {
                return Negotiate.WithView("image");
            };
            Get["/hidden"] = _ =>
            {
                return Negotiate.WithView("willNotBeFound");
            };
        }
    }
}