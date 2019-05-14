/* as2jTokenManager.cs */
namespace  Assets.Code.AsSyntax.parser {

public class as2jConstants {
public const int EOF  = 0;
public const int VAR = 7;
public const int TK_TRUE = 8;
public const int TK_FALSE = 9;
public const int TK_NOT = 10;
public const int TK_NEG = 11;
public const int TK_INTDIV = 12;
public const int TK_INTMOD = 13;
public const int TK_BEGIN = 14;
public const int TK_END = 15;
public const int TK_LABEL_AT = 16;
public const int TK_IF = 17;
public const int TK_ELSE = 18;
public const int TK_ELIF = 19;
public const int TK_FOR = 20;
public const int TK_WHILE = 21;
public const int TK_PAND = 22;
public const int TK_POR = 23;
public const int NUMBER = 24;
public const int STRING = 25;
public const int ATOM = 26;
public const int UNNAMEDVARID = 27;
public const int UNNAMEDVAR = 28;
public const int CHAR = 29;
public const int LETTER = 30;
public const int LC_LETTER = 31;
public const int UP_LETTER = 32;
public const int DIGIT = 33;
public static string[] tokenImage = { null, 
@" ", 
@"	", 
@"
", 
@"
", 
null, 
null, 
null, 
@"true", 
@"false", 
@"not", 
@"~", 
@"div", 
@"mod", 
@"begin", 
@"end", 
@"@", 
@"if", 
@"else", 
@"elif", 
@"for", 
@"while", 
@"|&|", 
@"|||", 
null, 
null, 
null, 
null, 
null, 
null, 
null, 
null, 
null, 
null, 
@"{", 
@"}", 
@":-", 
@".", 
@"!", 
@":", 
@"<-", 
@"+", 
@"-", 
@"^", 
@"?", 
@";", 
@"(", 
@")", 
@"!!", 
@"<", 
@">", 
@"::", 
@",", 
@"[", 
@"|", 
@"]", 
@"&", 
@"<=", 
@">=", 
@"==", 
@"\==", 
@"=", 
@"=..", 
@"*", 
@"/", 
@"**"};
public static string[] lexStateNames = {
"DEFAULT"
};
public const int DEFAULT = 0;
};
public partial class as2jTokenManager : as2jConstants {

protected bool MoveToNextChar() {
  try {
    curChar = input_stream.ReadChar();
  } catch(System.IO.IOException) {
    return false;
  }
  return true;
}



    /** Constructor. */
    public as2jTokenManager(ICharStream stream){

    input_stream = stream;
  }

  /** Constructor. */
  public as2jTokenManager (ICharStream stream, int lexState){
    ReInit(stream);
    SwitchTo(lexState);
  }

  /** Reinitialise parser. */
  
  public void ReInit(ICharStream stream)
  {


    jjmatchedPos = 0;
    curLexState = defaultLexState;
    input_stream = stream;
  }

  /** Reinitialise parser. */
  public void ReInit(ICharStream stream, int lexState)
  
  {
    ReInit(stream);
    SwitchTo(lexState);
  }

  /** Switch to specified lex state. */
  public void SwitchTo(int lexState) {
    curLexState = lexState;
  }

private  int JjRunStringLiteralMatch() {
  int curPos = 0;
  int key = (int)curLexState << 16 | curChar;
  int startState = jjInitStates[curLexState];
  if (startAndSize.ContainsKey(key)) {
    int[] arr = startAndSize[key];
    int index = arr[0];
    for (int i = 0; i < arr[1]; i++) {
      int len = stringLiterals[index++];
      do {
        if (curChar != stringLiterals[index + curPos]) break;
        if (++curPos == len) break;
        if (!MoveToNextChar()) {
          --curPos;
          break;
        }
      } while(curPos < len);
      if (curPos == len) {
        jjmatchedKind = stringLiterals[index + len];
        jjmatchedPos = curPos;
        startState = stringLiterals[index + len + 1];
        if (!MoveToNextChar()) {
          return curPos;
        }
        curPos++;
        break;
      } else {
        index += len + 2;
        input_stream.Backup(curPos + 1);
        curPos = 0;
        if (!MoveToNextChar()) {
          System.Diagnostics.Debug.Assert(false, "Unreachable code!");
        }
      }
    }
  } else {
  }
  return JjMoveNfa(startState, curPos);
}

private   int[] stateSet = new int[50];
private   int[] newStateSet = new int[50];
private   long[] moved = new long[50];
private  long moveIndex = 1L;

private  int JjMoveNfa(int startState, int curPos) {

  if (startState < 0) {
    return curPos;
  }

  // We have a long array indexed by the NFA state number to roughly indicate
  // the input position so when the input reaches part long.MaxValue (which
  // should be extremely rare), we need to reset them all back to zero.
  if (++moveIndex == long.MaxValue) {
    for (int i = 0; i < 50; i++) moved[i] = 0L;
    moveIndex = 1L;
  }

  // We initialize the kind to MAX value so that when a match is found, we can
  // simply check if it's less than the current match and store it in that
  // case. This helps implement the 'first occurring' rule properly.
  int cnt = 0;
  stateSet[cnt++] = startState;
  moved[startState] = moveIndex;
 
  // Some NFA states have epsilon transitions (move on empty string). So we
  // just start with all of them. Note that the nextStates array already adds
  // the epsilon closure. Only the initial state needs to do this explicitly.
  foreach (int s  in jjcompositeState[startState]) { 
    if (moved[s] != moveIndex) {
      stateSet[cnt++] = s;
      moved[s] = moveIndex;
    }
  }

  do {
    int newCnt = 0;
    int kind = int.MaxValue;
    if (++moveIndex == long.MaxValue) {
      for (int i = 0; i < 50; i++) moved[i] = 0L;
      moveIndex = 1L;
    }

    int vectorIndex = curChar >> 6;
    long bitpattern = (1L << (curChar & 0x3f));
    do {
      int state = stateSet[--cnt];
      if ((jjChars[state][vectorIndex] & bitpattern) != 0L) {
        // Current input character can move this NFA state. So add all the
        // next states of the current states for use with the next input char.
        foreach (int newState  in jjnextStateSet[state]) {
          if (moved[newState] != moveIndex) {
            // We add each state only once.
            newStateSet[newCnt++] = newState;
            moved[newState] = moveIndex;
          }
        }
        int newKind = jjmatchKinds[state];
        if (kind > newKind) {
          // It's a state so store the matched kind if it's smaller than
          // what's already matched.
          kind = newKind;
        }
      }
    } while (cnt > 0);

    if (kind != int.MaxValue) {
      // We found a match. So remember the kind and position of the match.
      jjmatchedKind = kind;
      jjmatchedPos = curPos;
      // Reset the kind to max value so we can contine looking for a longer
      // match.
      kind = int.MaxValue;
    }

    // Swap the current and next state sets.
    int[] tmp = stateSet;
    stateSet = newStateSet;
    newStateSet = tmp;
    // Reset the number of states.
    cnt = newCnt;
    if (newCnt == 0) {
      // There were no transitions from any of the current states on the
      // current input. So we are done.
      return curPos;
    }
    // Read the next character and try to continue running the NFA.
    if (!MoveToNextChar()) {
      // EOF reached!
      return curPos;
    }
    ++curPos;
  } while (cnt > 0);

  System.Diagnostics.Debug.Assert(false,
      "Interal error. Please submit a bug at: http://javacc.java.net.");
  return curPos;
}

private  int defaultLexState = 0;
private  int curLexState = 0;
private  int jjmatchedPos;
private  int jjmatchedKind;
private  string jjimage = string.Empty;
        private string image = ""; // string.Empty;
private int jjimageLen;
private int lengthOfMatch;
protected int curChar;
protected  ICharStream input_stream;

public static bool IsToken(int kind) {
  return (jjtoToken[kind >> 6] & (1L << (kind & 0x3f))) != 0L;
}

public static bool IsSkip(int kind) {
  return (jjtoSkip[kind >> 6] & (1L << (kind & 0x3f))) != 0L;
}

public static bool IsSpecial(int kind) {
  return (jjtoSpecial[kind >> 6] & (1L << (kind & 0x3f))) != 0L;
}

public static bool IsMore(int kind) {
  return (jjtoMore[kind >> 6] & (1L << (kind & 0x3f))) != 0L;
}

protected  Token JjFillToken() {
  Token t;
  string curTokenImage;
  int beginLine;
  int endLine;
  int beginColumn;
  int endColumn;
  if (jjmatchedPos < 0) {
    if (image == null) {
      curTokenImage = "";
    } else {
      curTokenImage = image.ToString();
    }
    beginLine = endLine = input_stream.GetEndLine();
    beginColumn = endColumn = input_stream.GetEndColumn();
  } else {
    string im = tokenImage[jjmatchedKind];
    curTokenImage = (im == null) ? input_stream.GetImage() : im;
    beginLine = input_stream.GetBeginLine();
    beginColumn = input_stream.GetBeginColumn();
    endLine = input_stream.GetEndLine();
    endColumn = input_stream.GetEndColumn();
  }

   t = Token.NewToken(jjmatchedKind);
   t.kind = jjmatchedKind;
   t.image = curTokenImage;

   t.beginLine = beginLine;
   t.endLine = endLine;
   t.beginColumn = beginColumn;
   t.endColumn = endColumn;

   return t;
}

/** Get the next Token. */
public  Token GetNextToken() {
  Token specialToken = null;
  Token matchedToken;
  int lastReadPosition = 0;

  EOFLoop:
  for (;;) {
    // First see if we have any input at all.
    try {
      curChar = input_stream.BeginToken();
    } catch(System.Exception) {
      // No input. So return EOF token.
      jjmatchedKind = EOF;
      jjmatchedPos = -1;
      matchedToken = JjFillToken();
      matchedToken.specialToken = specialToken;
      return matchedToken;
    }

    // Set matched kind to a MAX VALUE to implement largest, first occuring rule
    // i.e., smallest kind value matched should be used.
    image = jjimage;
    image = string.Empty;
    jjimageLen = 0;

    for (;;) {
      jjmatchedKind = int.MaxValue;
      jjmatchedPos = 0;
      lastReadPosition = JjRunStringLiteralMatch();
      if (jjmatchedPos == 0 && jjmatchedKind > canMatchAnyChar[curLexState]) {
        jjmatchedKind = canMatchAnyChar[curLexState];
      }

      if (jjmatchedKind != int.MaxValue) {
        // We have a match!
  
        // Put back any characters looked ahead.
        input_stream.Backup(lastReadPosition - jjmatchedPos);
        if (IsToken(jjmatchedKind)) {
          // Matched kind is a real TOKEN.
          matchedToken = JjFillToken();
          matchedToken.specialToken = specialToken;
          TokenLexicalActions(matchedToken);
          if (jjnewLexState[jjmatchedKind] != -1) {
            curLexState = jjnewLexState[jjmatchedKind];
          }
          return matchedToken;
        } else if (IsSkip(jjmatchedKind)) {
          // Matched kind is a SKIP or SPECIAL_TOKEN.
          if (IsSpecial(jjmatchedKind)) {
            matchedToken = JjFillToken();
            if (specialToken == null) {
              specialToken = matchedToken;
            } else {
              matchedToken.specialToken = specialToken;
              specialToken = (specialToken.next = matchedToken);
            }
            SkipLexicalActions(matchedToken);
          } else {
            SkipLexicalActions(null);
          }
          if (jjnewLexState[jjmatchedKind] != -1) {
            curLexState = jjnewLexState[jjmatchedKind];
          }
          goto EOFLoop;
        }
        // Here it's a MORE.
        MoreLexicalActions();
        if (jjnewLexState[jjmatchedKind] != -1) {
          curLexState = jjnewLexState[jjmatchedKind];
        }
        lastReadPosition = 0;
        jjmatchedKind = int.MaxValue;
        try {
          curChar = input_stream.ReadChar();
          continue;
        }
        catch (System.IO.IOException) { }
      }
      ReportError(lastReadPosition);
    }
  }
}

protected  void ReportError(int lastReadPosition) {
  int error_line = input_stream.GetEndLine();
  int error_column = input_stream.GetEndColumn();
  string error_after = null;
  bool EOFSeen = false;
  try {
    input_stream.ReadChar();
    input_stream.Backup(1);
  } catch (System.IO.IOException) {
    EOFSeen = true;
    error_after = lastReadPosition <= 1 ? "" : input_stream.GetImage();
    if (curChar == '\n' || curChar == '\r') {
       error_line++;
       error_column = 0;
    }
    else
       error_column++;
  }
  if (!EOFSeen) {
    input_stream.Backup(1);
    error_after = lastReadPosition <= 1 ? "" : input_stream.GetImage();
  }
  throw new TokenMgrError(EOFSeen, curLexState, error_line, error_column,
                          error_after, curChar, TokenMgrError.LEXICAL_ERROR);
}

private static readonly System.Collections.Generic.Dictionary<int, int[]> startAndSize =
    new System.Collections.Generic.Dictionary<int, int[]>();


private static readonly long[][] jjChars = new long[50][];
private static void InitNfaData() {
  for (int i = 0; i < 50; i++) {
    jjChars[i] = new long[1024];
    int ind = 0;
    // We originally generate something like RLE for the static arrays and
    // we actually expannd them here.
    for (int j = 0; j < jjCharData[i].Length; j += 2) {
      for (int k = 0; k < (int)jjCharData[i][j]; k++) {
        jjChars[i][ind++] = jjCharData[i][j + 1];
      }
    }
  }
}
private static readonly int[] stringLiterals = {
1, 64, 16, -1, 
1, 9, 2, -1, 
1, 10, 3, -1, 
1, 13, 4, -1, 
1, 91, 53, -1, 
3, 92, 61, 61, 60, -1, 
1, 93, 55, -1, 
1, 94, 43, -1, 
1, 32, 1, -1, 
2, 33, 33, 48, -1, 
1, 33, 38, -1, 
5, 98, 101, 103, 105, 110, 14, 48, 
3, 100, 105, 118, 12, 48, 
4, 101, 108, 105, 102, 19, 48, 
4, 101, 108, 115, 101, 18, 48, 
3, 101, 110, 100, 15, 48, 
5, 102, 97, 108, 115, 101, 9, 48, 
3, 102, 111, 114, 20, 48, 
1, 38, 56, -1, 
1, 40, 46, -1, 
2, 105, 102, 17, 48, 
1, 41, 47, -1, 
2, 42, 42, 65, -1, 
1, 42, 63, -1, 
1, 43, 41, -1, 
1, 44, 52, -1, 
3, 109, 111, 100, 13, 48, 
1, 45, 42, -1, 
3, 110, 111, 116, 10, 48, 
1, 46, 37, 49, 
1, 47, 64, 15, 
4, 116, 114, 117, 101, 8, 48, 
5, 119, 104, 105, 108, 101, 21, 48, 
2, 58, 58, 51, -1, 
2, 58, 45, 36, -1, 
1, 58, 39, -1, 
1, 123, 34, -1, 
1, 59, 45, -1, 
3, 124, 124, 124, 23, -1, 
3, 124, 38, 124, 22, -1, 
1, 124, 54, -1, 
2, 60, 61, 57, -1, 
2, 60, 45, 40, -1, 
1, 60, 49, -1, 
1, 125, 35, -1, 
3, 61, 46, 46, 62, -1, 
2, 61, 61, 59, -1, 
1, 61, 61, -1, 
1, 126, 11, -1, 
2, 62, 61, 58, -1, 
1, 62, 50, -1, 
1, 63, 44, -1};
static void InitStringLiteralData() {
startAndSize[64] = new int[]{0, 1};
startAndSize[9] = new int[]{4, 1};
startAndSize[10] = new int[]{8, 1};
startAndSize[13] = new int[]{12, 1};
startAndSize[91] = new int[]{16, 1};
startAndSize[92] = new int[]{20, 1};
startAndSize[93] = new int[]{26, 1};
startAndSize[94] = new int[]{30, 1};
startAndSize[32] = new int[]{34, 1};
startAndSize[33] = new int[]{38, 2};
startAndSize[98] = new int[]{47, 1};
startAndSize[100] = new int[]{55, 1};
startAndSize[101] = new int[]{61, 3};
startAndSize[102] = new int[]{81, 2};
startAndSize[38] = new int[]{95, 1};
startAndSize[40] = new int[]{99, 1};
startAndSize[105] = new int[]{103, 1};
startAndSize[41] = new int[]{108, 1};
startAndSize[42] = new int[]{112, 2};
startAndSize[43] = new int[]{121, 1};
startAndSize[44] = new int[]{125, 1};
startAndSize[109] = new int[]{129, 1};
startAndSize[45] = new int[]{135, 1};
startAndSize[110] = new int[]{139, 1};
startAndSize[46] = new int[]{145, 1};
startAndSize[47] = new int[]{149, 1};
startAndSize[116] = new int[]{153, 1};
startAndSize[119] = new int[]{160, 1};
startAndSize[58] = new int[]{168, 3};
startAndSize[123] = new int[]{182, 1};
startAndSize[59] = new int[]{186, 1};
startAndSize[124] = new int[]{190, 3};
startAndSize[60] = new int[]{206, 3};
startAndSize[125] = new int[]{220, 1};
startAndSize[61] = new int[]{224, 3};
startAndSize[126] = new int[]{239, 1};
startAndSize[62] = new int[]{243, 2};
startAndSize[63] = new int[]{252, 1};
}
private static readonly long[][] jjCharData = {
new long[] {1, 17179869184L},
new long[] {1, -17179878401L, 1, -268435457L, 1022, -1L},
new long[] {1, 0L, 1, 268435456L},
new long[] {1, 566935683072L, 1, 5700160604602368L},
new long[] {1, 17179869184L},
new long[] {1, 71776119061217280L},
new long[] {1, 71776119061217280L},
new long[] {1, 4222124650659840L},
new long[] {1, 71776119061217280L},
new long[] {1, 549755813888L},
new long[] {1, -549755813889L, 1023, -1L},
new long[] {1, 549755813888L},
new long[] {1, 287948901175001088L, 1, 576460745995190270L},
new long[] {1, 0L, 1, 576460743847706622L},
new long[] {1, 140737488355328L},
new long[] {1, 140737488355328L},
new long[] {1, -9217L, 1023, -1L},
new long[] {1, 9216L},
new long[] {1, 1024L},
new long[] {1, 8192L},
new long[] {1, 4398046511104L},
new long[] {1, -4398046511105L, 1023, -1L},
new long[] {1, 4398046511104L},
new long[] {1, -145135534866433L, 1023, -1L},
new long[] {1, -4398046511105L, 1023, -1L},
new long[] {1, 140737488355328L},
new long[] {1, 0L, 1, 134217726L},
new long[] {1, 287948901175001088L, 1, 576460745995190270L},
new long[] {1, 70368744177664L},
new long[] {1, 287948901175001088L},
new long[] {1, 0L, 1, 137438953504L},
new long[] {1, 43980465111040L},
new long[] {1, 287948901175001088L},
new long[] {1, 287948901175001088L, 1, 576460745995190270L},
new long[] {1, 70368744177664L},
new long[] {1, 0L, 1, 576460743713488896L},
new long[] {1, 0L, 1, 2147483648L},
new long[] {1, 287948901175001088L},
new long[] {1, 287948901175001088L, 1, 576460745995190270L},
new long[] {1, 287948901175001088L, 1, 576460745995190270L},
new long[] {1, 287948901175001088L},
new long[] {1, 287948901175001088L},
new long[] {1, 287948901175001088L},
new long[] {1, 70368744177664L},
new long[] {1, 287948901175001088L},
new long[] {1, 0L, 1, 137438953504L},
new long[] {1, 43980465111040L},
new long[] {1, 287948901175001088L},
new long[] {},
new long[] {}};
private static readonly int[][] jjcompositeState = {
new int[]{0, 9, 12, 13, 14, 26, 28, 35, 36, 40}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{15, 20}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{33, 34}, 
new int[]{29, 33}};
private static readonly int[] jjmatchKinds = {
2147483647, 
2147483647, 
2147483647, 
2147483647, 
25, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
26, 
29, 
30, 
2147483647, 
5, 
5, 
5, 
5, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
6, 
7, 
7, 
2147483647, 
24, 
2147483647, 
2147483647, 
24, 
26, 
2147483647, 
26, 
28, 
27, 
27, 
28, 
24, 
24, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
2147483647, 
24, 
2147483647, 
2147483647};
private static readonly int[][]  jjnextStateSet = {
new int[]{1, 2, 4}, 
new int[]{1, 2, 4}, 
new int[]{3, 5, 7}, 
new int[]{1, 2, 4}, 
new int[]{}, 
new int[]{1, 2, 4, 6}, 
new int[]{1, 2, 4}, 
new int[]{8}, 
new int[]{6}, 
new int[]{10, 11}, 
new int[]{10, 11}, 
new int[]{}, 
new int[]{}, 
new int[]{}, 
new int[]{15, 20}, 
new int[]{16, 17, 19}, 
new int[]{16, 17, 19}, 
new int[]{}, 
new int[]{}, 
new int[]{18}, 
new int[]{21, 22}, 
new int[]{21, 22}, 
new int[]{22, 23, 25}, 
new int[]{22, 24}, 
new int[]{22, 24}, 
new int[]{}, 
new int[]{27}, 
new int[]{27}, 
new int[]{29, 33}, 
new int[]{29, 30}, 
new int[]{31, 32}, 
new int[]{32}, 
new int[]{32}, 
new int[]{33, 34}, 
new int[]{33}, 
new int[]{33, 34}, 
new int[]{37, 39}, 
new int[]{37, 38}, 
new int[]{38}, 
new int[]{39}, 
new int[]{41, 42, 43, 44, 45}, 
new int[]{41}, 
new int[]{42, 43}, 
new int[]{29}, 
new int[]{44, 45}, 
new int[]{46, 47}, 
new int[]{47}, 
new int[]{47}, 
new int[]{}, 
new int[]{}};
private static readonly int[] jjInitStates  = {
0};
private static readonly int[] canMatchAnyChar = {
2147483647};
public static readonly string[] jjstrLiteralImages = {
null
, 
"\040", 
"\011", 
"\012", 
"\015", 
null
, 
null
, 
null
, 
"\0164\0162\0165\0145", 
"\0146\0141\0154\0163\0145", 
"\0156\0157\0164", 
"\0176", 
"\0144\0151\0166", 
"\0155\0157\0144", 
"\0142\0145\0147\0151\0156", 
"\0145\0156\0144", 
"\0100", 
"\0151\0146", 
"\0145\0154\0163\0145", 
"\0145\0154\0151\0146", 
"\0146\0157\0162", 
"\0167\0150\0151\0154\0145", 
"\0174\046\0174", 
"\0174\0174\0174", 
null
, 
null
, 
null
, 
null
, 
null
, 
null
, 
null
, 
null
, 
null
, 
null
, 
"\0173", 
"\0175", 
"\072\055", 
"\056", 
"\041", 
"\072", 
"\074\055", 
"\053", 
"\055", 
"\0136", 
"\077", 
"\073", 
"\050", 
"\051", 
"\041\041", 
"\074", 
"\076", 
"\072\072", 
"\054", 
"\0133", 
"\0174", 
"\0135", 
"\046", 
"\074\075", 
"\076\075", 
"\075\075", 
"\0134\075\075", 
"\075", 
"\075\056\056", 
"\052", 
"\057", 
"\052\052"};
private static readonly long[] jjtoSkip = {
126L, 8L};
private static readonly long[] jjtoSpecial = {
0L, 8L};
private static readonly long[] jjtoMore = {
0L, 8L};
private static readonly long[] jjtoToken = {
-127L, 11L};
private static readonly int[] jjnewLexState = {
-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
void TokenLexicalActions(Token matchedToken) {
  switch(matchedToken.kind) {
    case 26: {
        // Añadido image.Count > 0
        if ( image.Length > 0 && image [ 0 ] == '\'' ) matchedToken . image = image . Substring ( 1 , lengthOfMatch - 1 ) ;
      break;
    }
    default: break;
  }
}
void SkipLexicalActions(Token matchedToken) {
  switch(jjmatchedKind) {
    default: break;
  }
  switch(jjmatchedKind) {
    default: break;
  }
}
void MoreLexicalActions() {
jjimageLen += (lengthOfMatch = jjmatchedPos + 1);
  switch(jjmatchedKind) {
    default: break;
  }
}
static as2jTokenManager() {
  InitStringLiteralData();
  InitNfaData(); } 
}

}
