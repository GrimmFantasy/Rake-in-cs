using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rakentlk
{
    public enum Metric{

    //Different metrics that can be used for ranking.
    DEGREE_TO_FREQUENCY_RATIO = 0,  // Uses d(w)/f(w) as the metric
    WORD_DEGREE = 1, // Uses d(w) alone as the metric
    WORD_FREQUENCY = 2  // Uses f(w) alone as the metric
    }
    public class Rake
    {
        public HashSet<string> stopwords { get; set; } 
        public HashSet<string> punctuation { get; set; } 
        public string language { get; set; }
        public Metric ranking_metric { get; set; }
        public int max_length {get; set; }
        public int min_length {get; set; } 
        public bool include_repeat_phrase {get; set; }
        public List<string> sentence_tokenizer { get; set; }   
        public List<string> word_tokenizer { get; set; } 
        public Dictionary<string, int> frequency_dist { get; set; } = new();
        public Dictionary<string, int> degree { get; set; } = new();
        public List<Tuple<string, float>> rank_list { get; set; } = new();
        public List<string> ranked_phrases { get; set; }= new();

        public Rake(List<string> stopwords = default, List<string> punctuation = default, string language = "english", Metric ranking_metric = Metric.DEGREE_TO_FREQUENCY_RATIO, 
                int max_length = 100000, int min_length = 1, bool include_repeat_phrase = true)
        {
            if(stopwords == null) {
                this.stopwords = new HashSet<string>();

            }else{
                this.stopwords = stopwords.ToHashSet();
            }
            if(punctuation == null){
                this.punctuation = new HashSet<string>();
            }
            else{
                this.punctuation = punctuation.ToHashSet();
            }
            
            this.language = language;
            this.ranking_metric = ranking_metric;
            this.max_length = max_length;
            this.min_length = min_length;
            this.include_repeat_phrase = include_repeat_phrase;
        }

        public void extract_keywords_from_text(string text){
            List<string> sentences = this._tokenize_text_to_sentences(text);
            this.extract_keywords_from_sentences(sentences);
        }
        public void extract_keywords_from_sentences(List<string> sentences){
            List<List<string>> phrase_list = this._generate_phrases(sentences);
            this._build_frequency_dist(phrase_list);
            this._build_word_co_occurance_graph(phrase_list);
            this._build_ranklist(phrase_list);
        }
        public List<string> get_ranked_phrases_without_scores(){
            return this.ranked_phrases;
        }
        public List<Tuple<string, float>> get_ranked_phrases_with_scores(){
            return this.rank_list;
        }
        public Dictionary<string, int> get_word_frequency_distribution(){
            return this.frequency_dist;
        }
        public List<string> _tokenize_text_to_sentences(string text){
            
            return this.sentence_tokenizer = text.Split(". ").ToList();
        }
        public List<string> _tokenize_sentence_to_words(string sentence){
            return this.word_tokenizer = sentence.Split(' ', '.', ',').ToList();
        }
        public void _build_frequency_dist(List<List<string>> phrase_list){
            foreach(var phrase in phrase_list){
                    foreach(var word in phrase){

                    if(this.frequency_dist.ContainsKey(word)){
                        this.frequency_dist[word] +=1;
                    } else{
                    this.frequency_dist.Add(word, 1); 

                    }
                    }
            }
        }
        public void _build_word_co_occurance_graph(List<List<string>> phrase_list){
            Dictionary<string, Dictionary<string, int>> co_occurance_graph = new();
            System.Console.WriteLine("building occurance graph");
            foreach (var phrase in phrase_list){
                foreach(var word in phrase){
                    foreach(string coword in phrase){
                            //System.Console.WriteLine(coword + " " + word);
                        if(word != coword){
                            if(co_occurance_graph[word][coword] != null){
                                co_occurance_graph[word][coword]  +=1;
                            }else{  
                                Dictionary<string, int> _tmp = new();
                                _tmp.Add(word, 1);
                                co_occurance_graph.Add(word, _tmp);
                                _tmp.Clear();
                            }
                        }
                    }
                }
            }
            foreach (var set in co_occurance_graph)
            {
                System.Console.WriteLine("heres");
                System.Console.WriteLine( set.Key);
                if(this.degree[set.Key] != null){
                    this.degree[set.Key] = set.Value.Values.Sum();
                }else{
                    this.degree.Add(set.Key, set.Value.Values.Sum());
                }
            }
        }
        public void _build_ranklist(List<List<string>> phrase_list){
                float rank;
            foreach (var phrase in phrase_list)
            {
                rank = 0.0f;
                foreach (var word in phrase)
                {
                    System.Console.WriteLine(word);
                    System.Console.WriteLine(this.degree.Count());

                    if(this.ranking_metric == Metric.DEGREE_TO_FREQUENCY_RATIO){
                        rank += 1.0f * this.degree[word] / this.frequency_dist[word];
                    }else if(this.ranking_metric == Metric.WORD_DEGREE){
                        rank += 1.0f * this.degree[word];
                    }else{
                        rank += 1.0f * this.frequency_dist[word];
                    }
                }
                    Tuple<string, float> _tmp = new Tuple<string, float>(String.Join(' ', phrase), rank);
                    this.rank_list.Add(_tmp);
            }
            this.rank_list = this.rank_list.OrderByDescending(t => t.Item2).ToList();
            foreach (var sentence in rank_list)
            {
                this.ranked_phrases.Append(sentence.Item1);
            }
        }
        public List<List<string>> _generate_phrases(List<string> sentences){
            List<List<string>> phrase_list = new();
            foreach (var sentence in sentences)
            {
                List<string> word_list = new();
                foreach (var word in this._tokenize_sentence_to_words(sentence)){
                    word_list.Add(word.ToLower());
                }
                foreach (var phrase in this._get_phrase_list_from_words(word_list)){
                    phrase_list.Add(phrase);
                }
            }
            if(!this.include_repeat_phrase){
                HashSet<List<string>> unique_phrase_tracker = new();
                List<List<string>> non_repeated_phrase_list = new();
                foreach (var phrase in phrase_list)
                {
                    if(!unique_phrase_tracker.Contains(phrase)){
                        unique_phrase_tracker.Add(phrase);
                        non_repeated_phrase_list.Add(phrase);
                    }
                }
                return non_repeated_phrase_list;
            }
            else{
                return phrase_list;
            }
        }
        public List<List<string>> _get_phrase_list_from_words(List<string> word_list){
            List<Tuple<bool, List<string>>> groups = new();
            Tuple<bool, List<string>> _tmp;

            foreach(var word in word_list){
                if(this.punctuation.Contains(word) 
                    || this.stopwords.Contains(word)){
                        _tmp = new(false, new());
                        _tmp.Item2.Add(word);
                        groups.Add(_tmp);
                    }else{
                        _tmp = new(true, new());
                        _tmp.Item2.Add(word);
                        groups.Add(_tmp);
                        System.Console.WriteLine(word);
                    }        
            }
            List<List<string>> phrases = new();
            foreach (var group in groups)
            {
                if(group.Item1 
                    && (group.Item2.Count() >= this.min_length && group.Item2.Count() <= this.max_length)){
                    phrases.Add(group.Item2);
                    // foreach (var phrase in group.Item2){
                    //     foreach (var item in phrase)
                    //     {
                    //         System.Console.WriteLine(phrase);
                    //     }
                    // }

                }
            }
            return phrases;
        }
    }
}