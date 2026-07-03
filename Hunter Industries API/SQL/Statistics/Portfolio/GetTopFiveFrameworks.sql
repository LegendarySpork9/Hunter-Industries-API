select top 5
	[Name],
	count(*) as Uses
from Framework F with (nolock)
join PortfolioItemFramework PIF with (nolock) on F.FrameworkId = PIF.FrameworkId
group by [Name]
order by Uses desc