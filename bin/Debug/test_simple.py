# Note functions prefixed with 2 underscores as in: __Example() will not appear in the function list, but can be called normally.
# See HIDSuite Function List for methods available in the Host object.
# Note the python library is required for scripts attempting to import most things beyond clr, System, sys, and os.  (Install IronPython)
# ----------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------
# Do not modify the contents of this following block.  This should be the standard header used for all python scripts for hidsuite3.
global Host
Host = None     # necessary to enforce loading the attribute
import clr
import System
import sys
Math = System.Math
#sys.path.append(r'C:\ProgramData\Tripp Lite\hidsuite3\hidsuite3_stubs')     # This line is to support vscode's autocomplete
#sys.path.pop()          # undo the previous sys.path.append().  (The python parser doesn't pay attention to this line.))
#import Strings          # This is a stub file only to support autocomplete.
#from ScriptStubs import *
#from ScriptHelper import *
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.IO')
clr.AddReference('System')
clr.AddReference('Microsoft.VisualBasic')
from System.Windows.Forms import *      # MessageBox, MessageBoxButtons, DialogResult, etc.
from System.IO import *      # File.Exists, etc
from Microsoft.VisualBasic import *     # Strings
# __init__ is automatically called by hidsuite for the parent python script only upon loading the script.  All imported modules must be initialized within this __init__() function.


def __init__():
    pass

    # automatic Host initialization (Note: DO NOT ATTEMPT TO REFERENCE Host WITHIN __init__()):
#    mods = [m.__name__ for m in sys.modules.values() if m]
#    for mod in mods:
#        if (mod != 'Microsoft' and mod != 'clr' and mod != 'sys' and mod != 'zipimport' and mod != 'System' and not(mod.startswith('__'))):
#            module = sys.modules[mod]
#            if hasattr(module, 'Host'):
#                module.Host = Host      # initialize Host object
# ----------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------
# User scripts and import statements below this line:


def TEST1():
    #__init__()
    
    Host.Display("test Host.Display")

    for i in range(0, 5+1):
        Host.Display("test loop: " + str(i))

#include test disabled for now:
#    Display("test Display")
#    Display(include_test.Host)
#    include_test.HelloTest()
#    Display("---HelloTest() direct call below---")
#    HelloTest()

def TEST2():
    include_test2.HelloTest()

def PathTest():
    for paths in sys.path:
        Host.Display(paths)

def InitModules():
    mods = [m.__name__ for m in sys.modules.values() if m]
    for mod in mods:
        if (mod != "Microsoft" and mod != "clr" and mod != "sys" and mod != "zipimport" and mod != "System" and not(mod.startswith("__"))):
            Host.Display("Checking: " + mod)
            if (mod in globals()):
                module = globals()[mod]     #mod is just a string, so we need to retrieve the corresponding actual module object
                if hasattr(module, 'Host'):
                    Host.Display("Found " + mod + ".Host global.  Initializing now...")
                    module.Host = Host

    for mod in mods:
        if ("__init__" in dir(mod)):
            Host.Display(str(mod) + " has __init__()...  Running now.")
            mod.__init__()

    Host.Display("Done")

def TestStringToList():
    # testlist = ["be", "es", "fi", "ve"]
    testlist = "beesfivet"
    chunklist = _StringToList(testlist, 2)
    for i in chunklist:
        Host.Display(str(i))


def _StringToList(seq, size):
    """Takes input string <seq> and returns a list of <size>-character chunks.  If the string length is not an even multiple of <size>, the last list item will be the remaining characters and is not padded."""
    return list(seq[pos:pos + size] for pos in range(0, len(seq), size))


def TestListToString():
    testlist = [["12", 34, "56"], ["12", 34, "56"]]
    Host.Display(_ListToString(testlist))


def _ListToString(inputlist):
    """Concatenates the contents of <inputlist> into a single string."""
    return ''.join(str(item) for item in inputlist)


def TestInt():
    reports = int(0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF)
    Host.Display(reports)
    Host.Display(bin(reports))


def TestPadFunctionInput():
    teststring = "bees"
    padside = "B"
    fillchar = "S"
    length = 10
    Host.Display(_Pad(teststring, padside, fillchar, length))


def _Pad(instring, padside, fillchar, length):
    """Pads left, right, or both sides of a string with <fillchar> until string is <length> characters.  <padside> can be L/R/B for left/right/both."""
    if padside == "L": padside = ">"
    elif padside == "R": padside = "<"
    elif padside == "B": padside = "^"
    return "{0:{fill}{side}{length}}".format(instring, side=padside, fill=fillchar, length=length)


def TestSubstring():
    teststring = "123456"
    Host.Display("String: " + teststring)
    Host.Display(_Substring(teststring, 0, 3))      # Left 3
    Host.Display(_Substring(teststring, -1, 3))     # Mid 3, beginning at last character (essentially Right 1)
    Host.Display(_Substring(teststring, 0))     # Left, full string
    Host.Display(_Substring(teststring, -5, 3))     # Mid 3, beginning at 5th char from right
    Host.Display(_Substring(teststring, -5))    # Right 5
    Host.Display(_Substring(teststring, 7))     # Right (from 7)
    Host.Display(_Substring(teststring, -8))    # Right 8 (same as full string as length is less than 8)
    Host.Display(_Substring(teststring, -8, 1))     # Mid 1, beginning at 8th char from right.  Should return first character.
    Host.Display(_Substring(teststring, -6, 1))     # Mid 1, beginning at 8th char from right.  Should return first character.
    Host.Display(_Substring(teststring, -5, 1))     # Mid 1, beginning at 8th char from right.  Should return first character.


def _Substring(instring, start, length=None):
    """Returns a portion of the provided string beginning at the character with index <start> and continuing for <length> characters.  If length is not provided, will continue through the end of the string.  First character is index 0.  A negative start index can be provided to start at the specified distance from the end of the string rather than the beginning, with the last character being index -1."""
    # Host.Display("\r\nProvided start = {0} and length = {1}".format(start, length))
    end = None
    if (not length): end = None
    elif (start >= 0): end = start + length
    elif (start < 0):
        if (-start > len(instring)): start = -len(instring)
        end = len(instring) + start + length
    n = slice(start, end)
    # Host.Display("Results in slice from " + str(start) + " until " + str(end))
    return instring[n]


def TestH2D():
    Host.Display(H2D("FFFFFFFF"))
    Host.Display(int("FFFFFFFF", 16))
    Host.Display(Hex2Uint("FFFFFFFF"))


def TestStuffArrays():
    response = "1020304050607080"
    responselist = StringToList(response, 4)[::-1]
    for item in responselist: Host.Display(item)
    Host.Display("d")
    responselist[2:3] = [(str(item) + "X") for item in responselist[2:3]]
    for item in responselist: Host.Display(item)


def TestSetStringReport():
    newstring = "ABCD"
    Host.Display(ReverseHexStringOrder(ASCIIStringToHexString(newstring)))


def SetStringReport(reportID, asciistring, length=-1):
    """Converts a string of ASCII characters to hex bytes in reverse order
    and passes it to SetReport with the specified report ID and minimum length."""
    SetReport(reportID, ReverseHexStringOrder(ASCIIStringToHexString(asciistring)), length)


def Testsetbit():
    number = "0"
    for bit in range(64):
        item = setbithex(number, bit)
        Host.Display(number + "\t" + item)


def Testclrbit():
    number = "FFFFFFFFFFFFFFFF"
    for bit in range(64):
        item = clrbithex(number, bit)
        Host.Display(number + "\t" + item)

def RunTestMath():
    ntest._TestMath()


def RunTestSetBit():
    ntest.TestSetBit()
