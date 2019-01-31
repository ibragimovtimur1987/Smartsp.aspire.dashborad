using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models.Queries
{
    public class MSPCompareByCriteriaQuery
    {
        //1-я таблица данные
        internal static string QueryMspCompareByType(int region, int area, int category, int year)
        {
            var res = string.Format(@"
;with src (Name)
as
(
    select N'Адресный критерий' as Name
    union all
    select N'Категориальный критерий' as Name
),
aggr (Name, Beneficiaries, Expenditures)
as
(
	select N'Адресный критерий' as Name, sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Category"" c
        join public.""Indicators"" i on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        join public.""Values"" v on i.""Id"" = v.""Indicator""
        join public.""Regions"" r on r.""Id"" = v.""Region""
    where r.""Id"" = {0}
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
        and v.""TargetingMeansTested"" = true
        and v.""TargetingCategorical"" = false
    group by c.""Id""
    union all
    select N'Категориальный критерий' as Name, sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Category"" c
        join public.""Indicators"" i on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        join public.""Values"" v on i.""Id"" = v.""Indicator""
        join public.""Regions"" r on r.""Id"" = v.""Region""
    where r.""Id"" = {0}
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
        and v.""TargetingMeansTested"" = false
        and v.""TargetingCategorical"" = true
    group by c.""Id""
   ),
last(Name, Beneficiaries, Expenditures)
as
(
    select src.Name, aggr.Beneficiaries, aggr.Expenditures
    from src
        left outer join aggr on aggr.Name = src.Name
)
select* from last
union all
select N'Итого', sum(Beneficiaries), sum(Expenditures) from last
                ", region, area, category, year);
            return res;
        }

        internal static string QueryMspCompareByType(int[] regions, int area, int category, int year)
        {
            var res = String.Format(@"
;with src (Id, Name)
as
(
    SELECT r.""Id"", r.""Name""
    FROM public.""Regions"" r
    where r.""Id"" in ({0})
    order by r.""Id"", r.""Name""
),
aggrArd(Id, Name, Beneficiaries, Expenditures)
as
(
    select r.""Id"", r.""Name"", sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Regions"" r
        join public.""Values"" v on r.""Id"" = v.""Region""
        join public.""Indicators"" i on i.""Id"" = v.""Indicator""
        join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
    where r.""Id"" in ({0})
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
        and v.""TargetingMeansTested"" = true
        and v.""TargetingCategorical"" = false
    group by r.""Id"", r.""Name""
),
aggrCat (Id, Name, Beneficiaries, Expenditures)
as
(
    select r.""Id"", r.""Name"", sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Regions"" r
        join public.""Values"" v on r.""Id"" = v.""Region""
        join public.""Indicators"" i on i.""Id"" = v.""Indicator""
        join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
    where r.""Id"" in ({0})
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
        and v.""TargetingMeansTested"" = false
        and v.""TargetingCategorical"" = true
    group by r.""Id"", r.""Name""
),
aggrMix (Id, Name, Beneficiaries, Expenditures)
as
(

    select r.""Id"", r.""Name"", sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Regions"" r
        join public.""Values"" v on r.""Id"" = v.""Region""
        join public.""Indicators"" i on i.""Id"" = v.""Indicator""
        join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
    where r.""Id"" in ({0})
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
        and v.""TargetingMeansTested"" = true
        and v.""TargetingCategorical"" = true
    group by r.""Id"", r.""Name""
),
aggrSum (Id, Name, Beneficiaries, Expenditures)
as
(

    select r.""Id"", r.""Name"", sum(v.""Values""[1]) as Beneficiaries, sum(v.""Values""[5]) as Expenditures
    FROM public.""Regions"" r
        join public.""Values"" v on r.""Id"" = v.""Region""
        join public.""Indicators"" i on i.""Id"" = v.""Indicator""
        join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
    where r.""Id"" in ({0})
        and c.""Id"" = {2}
        and v.""DataYear"" = {3}
    group by r.""Id"", r.""Name""
),
last (Id, Name, Beneficiaries, Expenditures, Beneficiaries1, Expenditures1, Beneficiaries2, Expenditures2, Beneficiaries3, Expenditures3)
as
(
    select
        s.Id,
        s.Name,
        af.Beneficiaries as Beneficiaries,
        af.Expenditures as Expenditures,
        ar.Beneficiaries as Beneficiaries1,
        ar.Expenditures as Expenditures1,
        ac.Beneficiaries as Beneficiaries2,
        ac.Expenditures as Expenditures2,
        am.Beneficiaries as Beneficiaries3,
        am.Expenditures as Expenditures3
    from src s
        left outer join aggrArd af on s.Id = af.Id
        left outer join aggrCat ar on s.Id = ar.Id
        left outer join aggrMix ac on s.Id = ac.Id
        left outer join aggrSum am on s.Id = am.Id
)
select Id, Name, Beneficiaries, Expenditures, Beneficiaries1, Expenditures1, Beneficiaries2, Expenditures2, Beneficiaries3, Expenditures3 from last
                ", string.Join(",", regions), area, category, year);
            return res;
        }
    }
}