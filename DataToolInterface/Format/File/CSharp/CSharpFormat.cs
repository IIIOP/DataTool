namespace DataToolInterface.Format.File.CSharp
{
    public class CSharpFileAttribute : FileFormatAttribute
    {
        
    }
    
    public enum CSharpTypeNameEnum
    {
        CSharpFile,
        CSharpNameSpace,
        CSharpClass,
        CSharpEnum,
        CSharpStruct,
        CSharpAttribute,
        CSharpMethod,
        CSharpCtor,
        CSharpField,
        CSharpProperty,
        CSharpChunk,
        CSharpStatement,
        CSharpIntentStatement,
        CSharpBlock,
    }

    public enum CSharpFieldNameEnum
    {
        Head,
        Body,
        nameSpace,
        SpaceName,
        Usings,
        Classes,
        Enums,
        Structs,
        AttributeName,
        Arguments,
        Attributes,
        ClassName,
        StructName,
        Prefix,
        Postfixes,
        Ctor,
        Fields,
        Properties,
        Methods,
        EnumName,
        Items,
        FieldType,
        FieldName,
        InitExpression,
        PropertyType,
        PropertyName,
        MethodName,
        ReturnType,
        Get,
        Set,
    }
}