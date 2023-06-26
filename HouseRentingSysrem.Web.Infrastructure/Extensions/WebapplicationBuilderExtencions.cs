
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;


namespace HouseRentingSystem.Web.Infrastructure.Extensions
{
    public static  class WebapplicationBuilderExtencions
    {
            
        public static void AddApplicationServices(this IServiceCollection services,Type serviceType)
        {
            Assembly? serviseAssembly = Assembly.GetAssembly(serviceType);
            if (serviseAssembly==null)
            {
                throw new InvalidOperationException("Ivalid service");
            }
            Type[] serviceTypes = serviseAssembly.GetTypes().Where(t=>t.Name.EndsWith("Service")&& !t.IsInterface).ToArray();
            foreach (Type st in serviceTypes)
            {
                Type? interfaceType = st.GetInterface($"I{st.Name}");
                    if (interfaceType==null)
                    {
                    throw new InvalidOperationException("No interface is provide for service");
                     }
                services.AddScoped(interfaceType, st);
            }
          
        }
    }
}
