using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Config
{
	public interface IConfig
	{
		void ConfigureServices(IServiceCollection services);
		void Configure(IApplicationBuilder app);
	}
}
