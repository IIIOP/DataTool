namespace DataToolInterface.Format.File.Lua
{
    public class LuaFileAttribute : FileFormatAttribute
    {
        
    }

    public enum LuaTypeNameEnum
    {
        LuaFile,
        LuaChunk,
        LuaStatement,
        LuaBlock,
        LuaIntentBlock
    }

    public enum LuaFieldNameEnum
    {
        head,
        tail,
        body,
    }
}