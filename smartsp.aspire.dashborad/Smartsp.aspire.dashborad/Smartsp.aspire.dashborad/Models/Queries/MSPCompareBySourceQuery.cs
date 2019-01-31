using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models.Queries
{
    public class MSPCompareBySourceQuery
    {
        //1-ятаблица боковик
        public static string GetCategoriesTree(int[] areas)
        {
            var res = string.Format(@"
                    ;WITH RECURSIVE tree (Id, ParentId, Level, Name, SortOrder) as 
                    (
                     SELECT ""Id"", ""Parent"", 0 as level, ""Name"", ""SortOrder""

                        FROM public.""Category""
                     WHERE ""Parent"" IS NULL

                            and ""Id"" in ({0})

                       UNION ALL

                       SELECT c2.""Id"", c2.""Parent"", tree.Level + 1, c2.""Name"", ""SortOrder""
                       FROM public.""Category"" c2
                            INNER JOIN tree ON tree.Id = c2.""Parent""
                    )
                    SELECT Id, ParentId, Level, Name, SortOrder
                    FROM tree
                    order by SortOrder
                ", string.Join(",", areas));

            return res;
        }

        //1-я таблица данные
        internal static string QueryMspCompareBySource(int regions, int[] areas, int year)
        {
            var res = string.Format(@"
;WITH RECURSIVE tree (Id, ParentId, Level, Name, SortOrder) as 
(
	SELECT ""Id"", ""Parent"", 0 as level, ""Name"", ""SortOrder""

    FROM public.""Category""
	WHERE ""Parent"" IS NULL

        and ""Id"" in ({0})
	UNION ALL

    SELECT c2.""Id"", c2.""Parent"", tree.Level + 1, c2.""Name"", ""SortOrder""

    FROM public.""Category"" c2
    INNER JOIN tree ON tree.Id = c2.""Parent""
),
aggrFed(Id, Name, Expenditures)
as
(
	SELECT c.""Id"", c.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {1}
                and v.""DataYear"" = {2}

        and v.""FinSource"" = N'Федеральный'
        group by c.""Id"", c.""Name""
),
aggrReg(Id, Name, Expenditures)
as
(
	SELECT c.""Id"", c.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {1}
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Региональный'
        group by c.""Id"", c.""Name""
),
aggrComm(Id, Name, Expenditures)
as
(
	SELECT c.""Id"", c.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {1}
                and v.""DataYear"" = {2}

        and v.""FinSource"" = N'Софинансирование'
        group by c.""Id"", c.""Name""
),
aggr(Id, Level, Name, Federal, Regional, Common, SortOrder)
as(
	select t.Id, t.Level, t.Name, coalesce(af.Expenditures, 0) as Federal, coalesce(ar.Expenditures, 0) as Regional, coalesce(ac.Expenditures, 0) as Common, t.SortOrder
    from tree t

        left outer join aggrFed af on t.Id = af.Id
        left outer join aggrReg ar on t.Id = ar.Id
        left outer join aggrComm ac on t.Id = ac.Id
),
last(Id, Level, Name, Federal, Regional, Common, SortOrder)
as
(
	select 999999, 1, N'Итого' as Name, sum(aggr.Federal) as Federal, sum(aggr.Regional), sum(aggr.Common), 999999 
	from aggr

    where aggr.Level = 1
)
select Id, Name, Federal, Regional, Common, SortOrder from aggr
union all
select Id, Name, Federal, Regional, Common, SortOrder from last
order by SortOrder, Id
", string.Join(",", areas), regions, year);
            return res;
        }

        internal static string QueryMspCompareByType(int regions, int[] areas, int year)
        {
            var res = string.Format(@"
;with src (Name)
as
(
	select N'Федеральный бюджет (тыс.руб.)' as Name
	union all
	select N'Региональный бюджет (тыс.руб.)' as Name
	union all
	select N'Софинансирование (тыс.руб.)' as Name
),
aggrSS(Name, Expenditures)
as
(
	SELECT N'Федеральный бюджет (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}

        and v.""FinSource"" = N'Федеральный'

        and c.""Id"" = 62
        group by Name
        union all
        SELECT N'Региональный бюджет (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}

        and v.""FinSource"" = N'Региональный'

        and c.""Id"" = 62
        group by Name
    union all
        SELECT N'Софинансирование (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}

        and v.""FinSource"" = N'Софинансирование'

        and c.""Id"" = 62
        group by Name
),
aggrLM(Name, Expenditures)
as
(
	SELECT N'Федеральный бюджет (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Федеральный'

        and c.""Id"" = 72
        group by Name
    union all
    SELECT N'Региональный бюджет (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Региональный'

        and c.""Id"" = 72
        group by Name
    union all
    SELECT N'Софинансирование (тыс.руб.)' as Name, sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Category"" c
        join public.""Indicators"" i on i.""Area"" = c.""Id""
                join public.""Values"" v on i.""Id"" = v.""Indicator""
                join public.""Regions"" r on r.""Id"" = v.""Region""
        where r.""Id"" = {0}
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Софинансирование'
        and c.""Id"" = 72
        group by Name
), 
aggr(Name, SocialSupport, LaborMarket)
as
(
	select s.Name, ass.Expenditures, alm.Expenditures
    from src s
        left outer join aggrSS ass on s.Name = ass.Name
        left outer join aggrLM alm on s.Name = alm.Name
)
select Name, SocialSupport, LaborMarket from aggr
union all
select N'Итого (тыс. руб.)', sum(SocialSupport), sum(LaborMarket) from aggr
                ", regions, areas, year);
            return res;
        }

        internal static string QueryMspCompareByType(int[] regions, int area, int year)
        {
            var res = String.Format(@"
;with src (Id, Name)
as
(
	SELECT r.""Id"", r.""Name""
        FROM public.""Regions"" r
        where r.""Id"" in ({0})
        order by r.""Id""
),
aggrFed(Id, Name, Expenditures)
as
(
	SELECT r.""Id"", r.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Regions"" r
            join public.""Values"" v on r.""Id"" = v.""Region""
            join public.""Indicators"" i on i.""Id"" = v.""Indicator""
            join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        where r.""Id"" in ({0})
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Федеральный'
                and c.""Id"" = {1}
        group by r.""Id"", r.""Name""
),
aggrReg (Id, Name, Expenditures)
as
(
    SELECT r.""Id"", r.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Regions"" r
            join public.""Values"" v on r.""Id"" = v.""Region""
            join public.""Indicators"" i on i.""Id"" = v.""Indicator""
            join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        where r.""Id"" in ({0})
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Региональный'
                and c.""Id"" = {1}
        group by r.""Id"", r.""Name""
),
aggrCom (Id, Name, Expenditures)
as
(
    SELECT r.""Id"", r.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Regions"" r
            join public.""Values"" v on r.""Id"" = v.""Region""
            join public.""Indicators"" i on i.""Id"" = v.""Indicator""
            join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        where r.""Id"" in ({0})
                and v.""DataYear"" = {2}
                and v.""FinSource"" = N'Софинансирование'
                and c.""Id"" = {1}
        group by r.""Id"", r.""Name""
),
aggrSum (Id, Name, Expenditures)
as
(
    SELECT r.""Id"", r.""Name"", sum(v.""Values""[5]) as ""Expenditures""
        FROM public.""Regions"" r
            join public.""Values"" v on r.""Id"" = v.""Region""
            join public.""Indicators"" i on i.""Id"" = v.""Indicator""
            join public.""Category"" c on(i.""SubCategory"" = c.""Id"" or i.""Category"" = c.""Id"" or i.""Area"" = c.""Id"")
        where r.""Id"" in ({0})
                and v.""DataYear"" = {2}
                and c.""Id"" = {1}
        group by r.""Id"", r.""Name""
),
last (Id, Name, Federal, Regional, Common, Summ)
as
(
    select s.Id, s.Name, af.Expenditures as Federal, ar.Expenditures as Regional, ac.Expenditures as Common, am.Expenditures as Summ
    from src s
        left outer join aggrFed af on s.Id = af.Id
        left outer join aggrReg ar on s.Id = ar.Id
        left outer join aggrCom ac on s.Id = ac.Id
        left outer join aggrSum am on s.Id = am.Id
)
select Id, Name, Federal, Regional, Common, Summ from last
                ", string.Join(",", regions), area, year);
            return res;
        }
    }
}