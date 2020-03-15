using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Npgsql;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;

namespace TextCorpusSystem
{
    public class TextManager
    {
        static public void UploadText(StreamReader textStream, StreamReader annotationStream, string textName)
        {
            string text = textStream.ReadToEnd();
            string annotation = annotationStream.ReadToEnd();

            if (!IsValidAnnotation(new StringReader(annotation)))
                throw new InvalidAnnotationException();
            var tagMatches = ResolveAnnotation(annotationStream);

            using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                NpgsqlCommand lastTextIdQuery = new NpgsqlCommand("select last_value from texts_id_seq", db);
                long lastTextId = (long)lastTextIdQuery.ExecuteScalar();
                long curTextId = lastTextId == 1 ? 1 : lastTextId + 1;

                NpgsqlCommand uploadText = new NpgsqlCommand($"insert into texts (plaintext, annotation, textname) values ($${text}$$, $${annotation}$$, $${textName}$$)", db);
                try
                {
                    uploadText.ExecuteNonQuery();
                }
                catch
                {
                    throw new TextNameAlreadyExistException();
                }

                NpgsqlCommand getTagIdOffset = new NpgsqlCommand("select last_value from tags_id_seq", db);
                long tagIdOffset = (long)getTagIdOffset.ExecuteScalar();

                UploadTextSpanTags(tagMatches[0], db, curTextId);
                UploadANotes(tagMatches[1], db, tagIdOffset);
                UploadSharpNotes(tagMatches[2], db, tagIdOffset);
                UploadRelationTags(tagMatches[3], db, tagIdOffset);
            }

        }
    
        private static MatchCollection[] ResolveAnnotation(StreamReader annotationStream)
        {
            string textSpanTagPattern = @"(T\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s([0-9]+)\s([0-9]+)\s([^\n]+)";
            string aNotePattern = @"(A\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)";
            string sharpNotePattern = @"(#\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)\s([^\n]+)";
            string relationTagPattern = @"(R\d +)\s([a - zA - Zа - яА - Я0 - 9_] +)\sArg1: (T\d +)\sArg2: (T\d +)";

            

            annotationStream.DiscardBufferedData();
            annotationStream.BaseStream.Seek(0, SeekOrigin.Begin);
            string annotation = annotationStream.ReadToEnd();


            var textSpanTags = Regex.Matches(annotation, textSpanTagPattern);
            var aNotes = Regex.Matches(annotation, aNotePattern);
            var sharpNotes = Regex.Matches(annotation, sharpNotePattern);
            var relationTags = Regex.Matches(annotation, relationTagPattern);

            return new MatchCollection[] { textSpanTags, aNotes, sharpNotes, relationTags };
        }

        private static void UploadTextSpanTags(MatchCollection textSpanTags, NpgsqlConnection db, long curTextId)
        {
            foreach (Match match in textSpanTags)
            {
                NpgsqlCommand uploadTagName = new NpgsqlCommand();
                uploadTagName.Connection = db;
                uploadTagName.CommandText = $"insert into tagnames (tagname) values ('{match.Groups[2].Value}')";
                try
                {
                    uploadTagName.ExecuteNonQuery();
                }
                catch
                { }               

                NpgsqlCommand getTagNameId = new NpgsqlCommand($"select id from tagnames where tagname = '{match.Groups[2].Value}'", db);
                int tagNameId = (int)getTagNameId.ExecuteScalar();

                NpgsqlCommand uploadTextSpanTag = new NpgsqlCommand();
                uploadTextSpanTag.Connection = db;
                uploadTextSpanTag.CommandText = $"insert into tags (startpos, endpos, taggedtext, textid, nameid) values " +
                    $"({match.Groups[3].Value}, {match.Groups[4].Value}, $${match.Groups[5].Value}$$, '{curTextId}', '{tagNameId}')";
                uploadTextSpanTag.ExecuteNonQuery();
            }
        }

        private static void UploadANotes(MatchCollection ANotes, NpgsqlConnection db, long tagIdOffset)
        {
            foreach (Match match in ANotes)
            {
                NpgsqlCommand uploadTagName = new NpgsqlCommand();
                uploadTagName.Connection = db;
                uploadTagName.CommandText = $"insert into tagnames (tagname) values ('{match.Groups[2].Value}')";
                try
                {
                    uploadTagName.ExecuteNonQuery();
                }
                catch
                { }

                NpgsqlCommand getTagNameId = new NpgsqlCommand($"select id from tagnames where tagname = '{match.Groups[2].Value}'", db);
                int tagNameId = (int)getTagNameId.ExecuteScalar();

                long relatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));

                NpgsqlCommand uploadANote = new NpgsqlCommand();
                uploadANote.Connection = db;
                uploadANote.CommandText = $"insert into aNotes (nameId, tagId) values " +
                    $"({tagNameId}, '{relatedTagId}')";
                uploadANote.ExecuteNonQuery();
            }
        }

        private static void UploadSharpNotes(MatchCollection sharpNotes, NpgsqlConnection db, long tagIdOffset)
        {
            foreach (Match match in sharpNotes)
            {
                long relatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));
    
                NpgsqlCommand uploadSharpTag = new NpgsqlCommand();
                uploadSharpTag.Connection = db;
                uploadSharpTag.CommandText = $"insert into annotatorNotes (tagid, annotationtext) values " +
                    $"({relatedTagId}, $${match.Groups[4].Value}$$)";
                uploadSharpTag.ExecuteNonQuery();
            }
        }

        private static void UploadRelationTags(MatchCollection relationTags, NpgsqlConnection db, long tagIdOffset)
        {
            foreach (Match match in relationTags)
            {
                NpgsqlCommand uploadTagName = new NpgsqlCommand();
                uploadTagName.Connection = db;
                uploadTagName.CommandText = $"insert into tagnames (tagname) values ('{match.Groups[2].Value}')";
                try
                {
                    uploadTagName.ExecuteNonQuery();
                }
                catch
                { }

                NpgsqlCommand getTagNameId = new NpgsqlCommand($"select id from tagnames where tagname = '{match.Groups[2].Value}'", db);
                int tagNameId = (int)getTagNameId.ExecuteScalar();

                long firstRelatedTagId = tagIdOffset + int.Parse(match.Groups[3].Value.Trim('T'));
                long secondRelatedTagId = tagIdOffset + int.Parse(match.Groups[4].Value.Trim('T'));

                NpgsqlCommand uploadANote = new NpgsqlCommand();
                uploadANote.Connection = db;
                uploadANote.CommandText = $"insert into aNotes (nameId, firstTagId, secondTagId) values " +
                    $"({tagNameId}, {firstRelatedTagId}, {secondRelatedTagId})";
                uploadANote.ExecuteNonQuery();
            }
        }

        static private bool IsValidAnnotation(StringReader annotation)
        {
            string textSpanTagPattern = @"(T\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s([0-9]+)\s([0-9]+)\s([^\n]+)";
            string aNotePattern = @"(A\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)";
            string sharpNotePattern = @"(#\d+)\s([a-zA-Zа-яА-Я0-9_]+)\s(T\d+)\s([^\n]+)";
            string relationTagPattern = @"(R\d +)\s([a - zA - Zа - яА - Я0 - 9_] +)\sArg1:(T\d +)\sArg2:(T\d +)";

            for (string annotationLine = annotation.ReadLine(); annotationLine != null; annotationLine = annotation.ReadLine())
            {
                if (!(Regex.IsMatch(annotationLine, textSpanTagPattern) ||
                        Regex.IsMatch(annotationLine, aNotePattern) ||
                        Regex.IsMatch(annotationLine, sharpNotePattern) ||
                        Regex.IsMatch(annotationLine, relationTagPattern)))
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetText(int textId)
        {
            string text;
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                var getTextQuery = new NpgsqlCommand("select plaintext from texts where id = @textId", db);
                getTextQuery.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                getTextQuery.Parameters["@textId"].Value = textId;
                text = (string)getTextQuery.ExecuteScalar();
            }
            return text;
        }

        public static List<Tag> GetTags(int textId)
        {
            var tagList = new List<Tag>();
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                var getTagsQuery = new NpgsqlCommand("" +
                    "select tags.startpos, tags.endpos, tagnames.tagname " +
                    "from tags inner " +
                    "join tagnames on tagnames.id = tags.nameid " +
                    "where tags.textid = @textId", db);
                getTagsQuery.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                getTagsQuery.Parameters["@textId"].Value = textId;
                var tagsReader = getTagsQuery.ExecuteReader();


                if (tagsReader.HasRows)
                {
                    while (tagsReader.Read())
                    {
                        tagList.Add(new Tag((string)tagsReader.GetValue(2), (int)tagsReader.GetValue(0), (int)tagsReader.GetValue(1)));
                    }
                }
            }
            return tagList;
        }

        public static DataSet GetTextsDataSet()
        {
            DataSet textsDataSet = new DataSet();
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                NpgsqlDataAdapter textsDataAdapter = new NpgsqlDataAdapter("select textname, id from texts", db);
                textsDataAdapter.Fill(textsDataSet, "Texts");
            }
            return textsDataSet;
        }

        public static DataSet GetLemmaTagsDataSet()
        {
            DataSet tagsDataSet = new DataSet();
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                NpgsqlDataAdapter textsDataAdapter = new NpgsqlDataAdapter("select distinct on (tagname) tagname from lemmatags", db);
                textsDataAdapter.Fill(tagsDataSet, "lemmatags");
            }
            return tagsDataSet;
        }

        public static void DeleteText(int textId)
        {
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                NpgsqlCommand deleteTextCommand = new NpgsqlCommand("delete from texts where id = @textId", db);
                deleteTextCommand.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                deleteTextCommand.Parameters["@textId"].Value = textId;
                deleteTextCommand.ExecuteNonQuery();
            }
        }

        public static void UpdateTextAnnotation(int textId, StreamReader annotationStream)
        {
            string annotation = annotationStream.ReadToEnd();

            if (!IsValidAnnotation(new StringReader(annotation)))
                throw new InvalidAnnotationException();
            var tagMatches = ResolveAnnotation(annotationStream);

            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();

                NpgsqlCommand deleteOldTagsCommand = new NpgsqlCommand("delete from tags where textid = @textId", db);
                deleteOldTagsCommand.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                deleteOldTagsCommand.Parameters["@textId"].Value = textId;
                deleteOldTagsCommand.ExecuteNonQuery();

                NpgsqlCommand updateAnnotationCommand = new NpgsqlCommand("update texts set annotation = $$@newAnnotation$$ where id = @textId", db);
                updateAnnotationCommand.Parameters.Add("@textId", NpgsqlTypes.NpgsqlDbType.Integer);
                updateAnnotationCommand.Parameters["@textId"].Value = textId;
                updateAnnotationCommand.Parameters.Add("@newAnnotation", NpgsqlTypes.NpgsqlDbType.Text);
                updateAnnotationCommand.Parameters["@newAnnotation"].Value = annotation;
                updateAnnotationCommand.ExecuteNonQuery();

                NpgsqlCommand getTagIdOffset = new NpgsqlCommand("select last_value from tags_id_seq", db);
                long tagIdOffset = (long)getTagIdOffset.ExecuteScalar();

                UploadTextSpanTags(tagMatches[0], db, textId);
                UploadANotes(tagMatches[1], db, tagIdOffset);
                UploadSharpNotes(tagMatches[2], db, tagIdOffset);
                UploadRelationTags(tagMatches[3], db, tagIdOffset);
            }
        }
    }
}

