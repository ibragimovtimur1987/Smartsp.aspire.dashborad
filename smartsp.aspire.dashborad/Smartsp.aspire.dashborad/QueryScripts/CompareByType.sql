//1
;with cte
as
(
SELECT c."Id", c."Name", sum(v."Values"[1]) as "Beneficiaries" ,  sum(v."Values"[5]) as "Expenditures"
FROM public."Category" c
     join public."Indicators" i on i."SubCategory" = c."Id"
     join public."Values" v on i."Id" = v."Indicator"
     join public."Regions" r on r."Id" = v."Region"
where r."Id" = 69
      and i."Area" in (62, 72)
      and v."DataYear" = 2014
group by c."Id", c."Name"
), pres (Id, Name, Beneficiaries, Expenditures, SortOrder)
as
(  
select c."Id", c."Name", cte."Beneficiaries", cte."Expenditures", c."SortOrder" from public."Category" c
left outer join cte on cte."Id" = c."Id"
where c."Id" in (62, 72)    
) select Id, Name, Beneficiaries, Expenditures, SortOrder from pres
union all
select 999999, 'Итого' as Name, sum(Beneficiaries) as Beneficiaries, sum(Expenditures) as Expenditures, 999999 from pres
order by SortOrder, Id 

;with cte(Id, Name, Beneficiaries, Expenditures, SortOrder) as
(
    SELECT c."Id", c."Name", sum(v."Values"[1]) as "Beneficiaries", sum(v."Values"[5]) as "Expenditures", c."SortOrder"
	FROM public."Indicators" i
    	right join public."Category" as c on c."Id" = i."Category"
    	left join public."Values" as v on i."Id" = v."Indicator"
    where i."Area" = 62 and i."Region" = 66 and v."DataYear" = 2014 and c."Id" in (63, 69)
    group by c."Id", c."Name"
) select 2014 as DataYear, cte.Id, cte.Name, cte.Beneficiaries, cte.Expenditures, SortOrder from cte
union all
select 2014 as DataYear, 999999, N'Итого', sum(cte.Beneficiaries), sum(cte.Expenditures), 999999 from cte
order by SortOrder, Id


;with cte
as
(
SELECT r."Id", r."Name", sum(v."Values"[1]) as "Beneficiaries" ,  sum(v."Values"[5]) as "Expenditures"
FROM public."Regions" r
     join public."Values" v on r."Id" = v."Region"
     join public."Indicators" i on v."Indicator" = i."Id"
where r."Id" in (66,69)
      and i."Area" = 62
      and i."Category" in (63)
      and v."DataYear" = 2014
group by r."Id", r."Name"
) 
select r."Id", r."Name", cte."Beneficiaries", cte."Expenditures" from public."Regions" r
left outer join cte on cte."Id" = r."Id"
where r."Id" in (66,69)
                
;with cte
as
(
SELECT c."Id", c."Name", sum(v."Values"[1]) as "Beneficiaries" ,  sum(v."Values"[5]) as "Expenditures"
FROM public."Category" c
     join public."Indicators" i on i."SubCategory" = c."Id"
     join public."Values" v on i."Id" = v."Indicator"
     join public."Regions" r on r."Id" = v."Region"
where r."Id" = 69
      and i."Area" = 62
      and i."Category" = 62
      and i."SubCategory" in (62,63)
      and v."DataYear" = 2014
group by c."Id", c."Name"
), pres (Id, Name, Beneficiaries, Expenditures, SortOrder)
as
(  
select c."Id", c."Name", cte."Beneficiaries", cte."Expenditures", c."SortOrder" from public."Category" c
left outer join cte on cte."Id" = c."Id"
where c."Id" in (62, 63)    
) select Id, Name, Beneficiaries, Expenditures, SortOrder from pres
union all
select 999999, 'Итого' as Name, sum(Beneficiaries) as Beneficiaries, sum(Expenditures) as Expenditures, 999999 from pres
order by SortOrder, Id  




