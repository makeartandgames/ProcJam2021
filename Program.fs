open System
open FSharpPlus
open Rant
(* 
Might need to re-export (unpack) the dictionary to see what data is in there. 

Generate an Inform 7 game, then query the game, then use that advanced taxonomy to generate another game
hop between n-gram markoved corpora mid generation chain

generatean inform game using predicates extractedfrom a given (small) corpus. 
Extract terms and some simple relations.
*)
// let RANTIONARY_FILE = "Rantionary-4.0.0.rantpkg"
let RANTIONARY_FILE = "Rantionry-3.0.17.rantpkg"
// #r "nuget: Rant";;
// open Rant . . . 
// Ctrl+P -> FSI: Send file
(*
  ::=x for variable storage i.e.
<noun::=a> <noun::=a.pl>

. . . <filler> . . . 

<color.ish>
can't naively regex out '/' without fucking up pron tag
gradability? I think t is not built-in?
how to filter on metadata properties? 

[capsinfer:The Blue Heron{<noun> <verb-intransitive}] => The Highway Waste
 
Welcome{.|,{dear|<adj-appearance>|\s} traveller} 
It might be nice to dump an Inform game's index as a rant taxonomy, allowing:
  He wore <noun-thing-wearable> given that wearable is wearable is defined by inform
He came with his <adj-appearance::=a> <noun> his <adj::=a> <noun> his <adj::=a> <noun-concept>
his <noun-tool> wrapped in <noun-surface>
[case:sentence]
[chance:20]{}
?make if this  and if I do, don't make this one
           |                               |
   The {<adjective>|??=a} <noun-!person>{ which is <adj>|, <adjective>}
this is how:  "The [x:A;locked]{||<adj>} <noun> [x:A;locked]{ which is <adj>|, <adj>|}";; . . with synchronizer block attribute. x is just short-hand for `sync`
  |   id  
[sync:A;<type>]
the blocks get IDs which makes them keep state 
The following will always print A, B, C
R "[sync:A;forward]{A}, [sync:A;forward]{A|B}, [sync:A;forward]{A|B|C}"

@ makes it use the same class. (as opposed to = which si same thign)
R "<adj::@A> <noun::@B>, which was more <adj::@A> than <noun::@B>"
There is also @=, which matches all its classes, @+ which I don't understand
rhyme carrier: &  . . . 
not sure if it's possible to use multiple carriers; probably via qcc which has multiple carriers
[rhyme:pararhyme] <- but rhyme may not be implemented or I may be missing pronounciation info
[rs:n;...] 
R "[case:sentence]{[rs:4; . . . ]{the <noun-surface>}}";;
R "[case:sentence]{[rs:4;\s. . .\s ]{the <noun-surface>}} {each|all} <verb-intransitive.ing> [x:P;locked]{from| past} [x:P;locked]{{beyond|far|}|{this|here}}";; 
  => "The bed . . .  The ceiling . . .  The bedsheet . . .  The desk each sputtering past here"
  replacers: not sure how they'll be useful.
  subroutines, defined with $, allow you to sub in w/e 
  how do I query non-class metadata? i.e. the species example
R "  [$[greet:name]: {hello|greetings}, [arg:name]{.|...|!}] [case:sentence]{[$greet:<name>]} "
variables, tough I'm not sure how they're useful

R "[case:sentence]{[rs:4;\s. . .\s ]{the <noun-surface ? `.*a.*`::!A>}} {each|all} <adj-appearance> . . .  {each|all} <verb-intransitive.ing> [x:P;locked]{from| past} [x:P;locked]{{beyond|far|}|{this|here}}";; 
{[rs:4;\s. . .\s ]{the <noun-surface ? `.*a.*`::!A>}} <= !A guarantees different word, but each will match the regex.
#r "nuget:Rant";;
open Rant;;
let r = new RantEngine();;
r.LoadPackage("Rantionary-3.0.17.rantpkg")
let R x = r.Do(RantProgram.CompileString(x))
R "" 
R "The [x:A:locked]{||(.7) <adjective>} [x:A:locked]{ which is <adj>|, <adj>|}"
*)
// "He came with his <adj-appearance ? `l.*`::=a> <noun-weapon ? `l.*`> his <adj::=a> <noun-tool> wrapped in <adj::=a> <noun-surface> 
(*
// R "The [x:A;locked]{||<adj>} <noun> [x:A;locked]{ which is <adj>|, <adj>|}"
might be a way to regex on the pronounciation? 
"His <adj-appearance ? `^l.*`::=a> <noun-tool ? `^l[^\s]+$`> his <adj::=a> <noun-weapon ? `^l.*`>"
Alliteration:
[qname:word1;adj]                   # Table name
[qcf:word1;appearance]              # Filter for appearance-related adjectives
[qhas:word1;^[branch:ml;\c];i]      # Regex filter

# Create another one called word2 for the noun
[qname:word2;noun]                  # Table name
[qsub:word2;pl]                     # Subtype
[qhas:word2;^[branch:ml;\c];i]      # Regex filter
lfilter built in function

{(weight) a | (weight2) b} => {(.01)A|(.99)B}

R "[x:foo;locked]{Foo|Bar}\s [x:foo;locked]{Foo|Bar}" 
*)
let from whom =
    let s = head (map string [|2;3;4;5|])
    sprintf "from %s" s
let arr =  [|"this $product is <adj>.";
               "I tried to <verb-violent> it but got <noun-food> all over it.";
               "i use it <timeadv-frequency> when i'm in my <noun-place-indoor>.";
               "My <noun-living-animal> loves to play with it.";
               "[vl:ending][ladd:ending;!;!!;!!!;.]The box this comes in is [num:3;5] <unit-length> by [num:5;6] <unit-length> and weights [num:10;20] <unit-weight>[lrand:ending]";
               "This $product works <advattr> well. It <adv> improves my <activity> by a lot.";
               "I saw one of these in <country> and I bought one.";
               "one of my hobbies is <hobby::=A>. and when i'm <hobby.pred::=A> this works great.";
               "It only works when I'm <country>.";
               "My neighbor <name-female> has one of these. She works as a <noun-living-job> and she says it looks <adj-appearance>.";
               "My co-worker <name-male> has one of these. He says it looks <adj-appearance>.";
               "heard about this on <musicgenre> radio; decided to give it a try.";
               "[vl:ending][ladd:ending;!;!!;!!!;.]talk about <noun-uc-emotion>[lrand:ending]"|]
let compile = RantProgram.CompileString
let initRant = 
  let r = new RantEngine()
  r.LoadPackage(RANTIONARY_FILE)
  r

let ranter = 
  let r = new RantEngine()
  r.LoadPackage(RANTIONARY_FILE)
  let p1 = "[case:title]<noun> {(0.3)FC|(0.3)United|}"
  let program = RantProgram.CompileString(p1);
  let xs = map (fun x -> r.Do(compile(x)).Main) arr
  let s = intersperse "\n" xs |> sum 
  s //let z: List<string> = map (fun (x:RantProgram) -> x.Main)  lines 
  //let t = r.Do(program)
  // t.Main
// let toPigLatin (word: string) =
//     let isVowel (c: char) =
//         match c with
//         | 'a' | 'e' | 'i' |'o' |'u'
//         | 'A' | 'E' | 'I' | 'O' | 'U' -> true
//         |_ -> false
    
//     if isVowel word[0] then
//         word + "yay"
//     else
//         word[1..] + string word[0] + "ay"
[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s : %s" message ranter
    0 // return an integer exit code