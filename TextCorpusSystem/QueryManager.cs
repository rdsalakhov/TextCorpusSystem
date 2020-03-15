using Npgsql;
using System.Web.UI;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCorpusSystem
{
    public class QueryManager
    {
        static int baseDisplayOffset = 90;
        public static List<QueryResult> FindExactForm(string exactForm)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                string commandString = $@"
select distinct on (startPos, endPos)
textname,
plaintext,
taggedtext,
startPos,
endPos
from tags INNER JOIN texts on texts.id = tags.textid
where taggedtext similar to $${exactForm}_$$";
                NpgsqlCommand query = new NpgsqlCommand(commandString, db);
                query.Parameters.Add("@offset", NpgsqlTypes.NpgsqlDbType.Integer);
                query.Parameters.Add("@exactForm", NpgsqlTypes.NpgsqlDbType.Varchar);
                query.Parameters["@offset"].Value = baseDisplayOffset;
                query.Parameters["@exactForm"].Value = exactForm;

                NpgsqlDataReader queryReader = query.ExecuteReader();

                if (queryReader.HasRows)
                {
                    while (queryReader.Read())
                    {
                        var startPositions = new List<int> { queryReader.GetInt32(3) };
                        var endPositions = new List<int> { queryReader.GetInt32(4) };
                        queryResults.Add(new QueryResult(queryReader.GetString(0), queryReader.GetString(1), startPositions, endPositions));
                    }
                }

            }

            return queryResults;
        }

            public static List<QueryResult> FindTagPairAtRange(string firstTagName, string secondTagName, int inRange, int outRange)
            {
                var queryResults = new List<QueryResult>();
                using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    db.Open();

                    string createViewsCommandString = $@"
create or replace view firstTagName as 
select * from lemmatags
where tagname = $${firstTagName}$$;
create or replace view secondTagName as
select* from lemmatags
where tagname = $${secondTagName}$$;";
                    NpgsqlCommand createSupportiveViewsCommand = new NpgsqlCommand(createViewsCommandString, db);
                    //createSupportiveViewsCommand.Parameters.Add("@firstTagName", NpgsqlTypes.NpgsqlDbType.Text);
                    //createSupportiveViewsCommand.Parameters.Add("@secondTagName", NpgsqlTypes.NpgsqlDbType.Text);
                    //createSupportiveViewsCommand.Parameters["@firstTagName"].Value = firstTagName;
                    //createSupportiveViewsCommand.Parameters["@secondTagName"].Value = secondTagName;
                    createSupportiveViewsCommand.ExecuteNonQuery();

                    string findPairsQueryString = $@"
select 
textname, plaintext,
ftn.startpos, ftn.endpos,
stn.startpos, stn.endpos 
from firstTagName as ftn
join secondTagName as stn on ftn.textid = stn.textid 
join texts on texts.id = ftn.textid
where @(ftn.wordindex - stn.wordindex) <= @outRange
and @(ftn.wordindex - stn.wordindex) >= @inRange
";
                    NpgsqlCommand findPairsQuery = new NpgsqlCommand(findPairsQueryString, db);
                    findPairsQuery.Parameters.Add("@inRange", NpgsqlTypes.NpgsqlDbType.Integer);
                    findPairsQuery.Parameters.Add("@outRange", NpgsqlTypes.NpgsqlDbType.Integer);
                    findPairsQuery.Parameters["@inRange"].Value = inRange;
                    findPairsQuery.Parameters["@outRange"].Value = outRange;

                    NpgsqlDataReader queryReader = findPairsQuery.ExecuteReader();

                    if (queryReader.HasRows)
                    {
                        int i = 0;
                        while (queryReader.Read())
                        {                           
                            var startPositions = new List<int> { queryReader.GetInt32(2), queryReader.GetInt32(4) };
                            var endPositions = new List<int> { queryReader.GetInt32(3), queryReader.GetInt32(5) };
                            queryResults.Add(new QueryResult(queryReader.GetString(0), queryReader.GetString(1), startPositions, endPositions));
                            i++;
                        }
                    }
                }
                return queryResults;
            }

            public static List<QueryResult> FindLemmaPairAtRange(string firstLemma, string secondLemma, int inRange, int outRange)
            {
                var queryResults = new List<QueryResult>();
                using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    db.Open();

                    string createViewsCommandString = $@"
create or replace view firstLemma as
select * from lemmatags
where lemma like $${firstLemma}$$
or lemma like $$%|{firstLemma}$$
or lemma like $${firstLemma}|%$$;

create or replace view secondLemma as
select * from lemmatags
where lemma like $${secondLemma}$$
or lemma like $$%|{secondLemma}$$
or lemma like $${secondLemma}|%$$;
";
                    NpgsqlCommand createSupportiveViewsCommand = new NpgsqlCommand(createViewsCommandString, db);
                    createSupportiveViewsCommand.ExecuteNonQuery();

                    string findPairsQueryString = $@"
select 
textname, plaintext,
fl.startpos, fl.endpos,
sl.startpos, sl.endpos 
from firstLemma as fl
join secondLemma as sl on fl.textid = sl.textid 
join texts on texts.id = fl.textid
where @(fl.wordindex - sl.wordindex) <= @outRange
and @(fl.wordindex - sl.wordindex) >= @inRange
";
                    NpgsqlCommand findPairsQuery = new NpgsqlCommand(findPairsQueryString, db);
                    findPairsQuery.Parameters.Add("@inRange", NpgsqlTypes.NpgsqlDbType.Integer);
                    findPairsQuery.Parameters.Add("@outRange", NpgsqlTypes.NpgsqlDbType.Integer);
                    findPairsQuery.Parameters["@inRange"].Value = inRange;
                    findPairsQuery.Parameters["@outRange"].Value = outRange;

                    NpgsqlDataReader queryReader = findPairsQuery.ExecuteReader();

                    if (queryReader.HasRows)
                    {
                        while (queryReader.Read())
                        {
                            var startPositions = new List<int> { queryReader.GetInt32(2), queryReader.GetInt32(4) };
                            var endPositions = new List<int> { queryReader.GetInt32(3), queryReader.GetInt32(5) };
                            queryResults.Add(new QueryResult(queryReader.GetString(0), queryReader.GetString(1), startPositions, endPositions));
                        }
                    }
                }
                return queryResults;
            }
    }
} 
