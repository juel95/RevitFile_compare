using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace RevitFile_compare
{
    public static class ExtensibleStorage
    {
        public static void AddEntityData(this Element elem,Document doc, string entityFileName = "TS_Mark", string data = "mark")
        {
            doc.Invoke(m =>
            {
                SchemaBuilder schemaBuilder =new SchemaBuilder(new Guid("11111111-aaaa-2222-bbbb-333333333333"));
                // allow anyone to read the object 
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                // restrict writing to this vendor only 
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                // create a field to store an XYZ 
                FieldBuilder fieldBuilder =schemaBuilder.AddSimpleField(entityFileName, typeof(string));
                //fieldBuilder.SetUnitType(UnitType.UT_Length);
                fieldBuilder.SetDocumentation("A mark for TS");
                schemaBuilder.SetSchemaName(data);
                Schema schema = schemaBuilder.Finish(); // register the Schema object 
                // create an entity (object) for this schema (class) 
                Entity entity = new Entity(schema);
                // get the field from the schema 
                Field fieldSpliceLocation = schema.GetField(entityFileName);
                entity.Set<string>(fieldSpliceLocation, data); // set the value for this entity 
                elem.SetEntity(entity); // store the entity in the element 
            });

        }
        /// <summary>
        /// 判断扩展数据文件夹中标记数据是否存在
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="entityFileName">扩展数据文件夹名</param>
        /// <param name="data">扩展数据标记</param>
        /// <returns></returns>
        public static bool IsExistEntityData(this Element elem, string entityFileName= "TS_Mark",string data="mark")
        {
            var guids = elem.GetEntitySchemaGuids();
            if(guids?.Count>0)
            {
                for(int i=0;i<guids.Count;i++)
                {
                    Schema schema = Schema.Lookup(guids[i]);
                    Entity entity = elem.GetEntity(schema);
                    if (entity.Get<string>(schema.GetField("TS_Mark")) == "mark")  //从元素获取数据
                        return true;
                }
            }
            return false;
        }

    }
}
