#Запрос на поиск точной формы слова
select distinct on (startPos, endPos)
textname,
plaintext,
taggedtext,
startPos,
endPos
from tags INNER JOIN texts on texts.id = tags.textid
where taggedtext similar to $${exactForm}_$$


# Представление отображающее все леммы их номер в текстах
create or replace view lemmatags as
select tagid, startPos, endPos, taggedText,
textId, tagname, lemma,
t1.id - (select min(t2.id) from tags as t2 where t1.textId = t2.textId) as wordIndex
from tags as t1
join tagnames
on nameid = tagnames.id
join (select tagid, substring(annotatorStrings.annotationText from 'lemma = ''#"%#"''%' for '#') as lemma
	from (select tagid, annotationText from annotatorNotes) as annotatorStrings
	where substring(annotatorStrings.annotationText from 'lemma = ''#"%#"''%' for '#') is not null) as lemmas 
on tagid = t1.id

# Вспомогательные представления для поиска пары тегов на заддном расстоянии друг от друга (одноразовые)
create view firstTagName as 
select * from lemmatags
where tagname = '{tagname}'

create view secondTagName as
select * from lemmatags
where tagname = '{tagname}'

# Запрос на получение всех пар тегов на заданном расстоянии друг от друга 
select * from firstTagName as ftn
join secondTagName as stn on ftn.textid = stn.textid 
where @(ftn.wordindex - stn.wordindex) <= {wordCount}