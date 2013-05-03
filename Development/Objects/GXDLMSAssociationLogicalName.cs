//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.ManufacturerSettings;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public enum AssociationStatus
    {
        NonAssociated = 0,
        AssociationPending = 1,
        Associated = 2
    }

    public class GXDLMSAssociationLogicalName : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSAssociationLogicalName()
            : this("0.0.40.0.0.255")
        {
        }

         /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSAssociationLogicalName(string ln)
            : base(ObjectType.AssociationLogicalName, ln, 0)
        {
            ObjectList = new GXDLMSObjectCollection();
        }

        [XmlIgnore()]
        [GXDLMSAttribute(2, Static = true)]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            private set;
        }


        [XmlIgnore()]
        [GXDLMSAttribute(3)]
        public object AssociatedPartnersId
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(4)]
        public object ApplicationContextName
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(5)]
        public object XDLMSContextInfo
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(6)]
        public object AuthenticationMechanismMame
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(7)]
        public object Secret
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(8, Access = AccessMode.Read)]
        public AssociationStatus AssociationStatus
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(9, Access = AccessMode.Read)]
        public string SecuritySetupReference
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, ObjectList, AssociatedPartnersId, ApplicationContextName,
            XDLMSContextInfo, AuthenticationMechanismMame, Secret, AssociationStatus, SecuritySetupReference};
        }

        #region IGXDLMSBase Members

        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 9;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 4;
        }        

     /// <summary>
     /// Returns Association View.    
     /// </summary>     
    private byte[] GetObjects()
    {
        List<byte> stream = new List<byte>();
        stream.Add((byte)DataType.Array);
        bool lnExists = ObjectList.FindByLN(ObjectType.AssociationShortName, this.LogicalName) != null;
        //Add count        
        int cnt = ObjectList.Count();
        if (!lnExists)
        {
            ++cnt;
        }
        GXCommon.SetObjectCount(cnt, stream);
        foreach (GXDLMSObject it in ObjectList)
        {
            stream.Add((byte) DataType.Structure);
            stream.Add((byte) 4); //Count
            GXCommon.SetData(stream, DataType.UInt16, it.ObjectType); //ClassID
            GXCommon.SetData(stream, DataType.UInt8, it.Version); //Version
            GXCommon.SetData(stream, DataType.OctetString, it.LogicalName); //LN
            GetAccessRights(it, stream); //Access rights.
        }
        if (!lnExists)
        {
            stream.Add((byte) DataType.Structure);
            stream.Add((byte) 4); //Count
            GXCommon.SetData(stream, DataType.UInt16, this.ObjectType); //ClassID
            GXCommon.SetData(stream, DataType.UInt8, this.Version); //Version
            GXCommon.SetData(stream, DataType.OctetString, this.LogicalName); //LN
            GetAccessRights(this, stream); //Access rights.
        }
        return stream.ToArray();    
    }

    private void GetAccessRights(GXDLMSObject item, List<byte> data) 
    {
        data.Add((byte)DataType.Structure);
        data.Add((byte)2);
        data.Add((byte)DataType.Array);
        GXAttributeCollection attributes = item.Attributes;        
        int cnt = (item as IGXDLMSBase).GetAttributeCount();
        data.Add((byte)cnt);        
        for (int pos = 0; pos != cnt; ++pos)            
        {
            GXDLMSAttributeSettings att = attributes.Find(pos + 1);
            data.Add((byte)DataType.Structure); //attribute_access_item
            data.Add((byte)3);
            GXCommon.SetData(data, DataType.Int8, pos + 1);
            ///If attribute is not set return read only.
            if (att == null)
            {
                GXCommon.SetData(data, DataType.Enum, AccessMode.Read);
            }
            else
            {
                GXCommon.SetData(data, DataType.Enum, att.Access);
            }
            GXCommon.SetData(data, DataType.None, null);
        }
        data.Add((byte)DataType.Array);
        attributes = item.MethodAttributes;
        cnt = (item as IGXDLMSBase).GetMethodCount();
        data.Add((byte)cnt);
        for (int pos = 0; pos != cnt; ++pos)            
        {
            GXDLMSAttributeSettings att = attributes.Find(pos + 1);
            data.Add((byte)DataType.Structure); //attribute_access_item
            data.Add((byte)2);
            GXCommon.SetData(data, DataType.Int8, pos + 1);
             ///If method attribute is not set return no access.
            if (att == null)
            {
                GXCommon.SetData(data, DataType.Enum, MethodAccessMode.NoAccess);
            }
            else
            {
                GXCommon.SetData(data, DataType.Enum, att.MethodAccess);                
            }
        }        
    }         

    void UpdateAccessRights(GXDLMSObject obj, Object[] buff)
    {
        foreach (Object[] attributeAccess in (Object[])buff[0])
        {            
            int id = Convert.ToInt32(attributeAccess[0]);
            int mode = Convert.ToInt32(attributeAccess[1]);
            obj.SetAccess(id, (AccessMode)mode);
        }
        foreach (Object[] methodAccess in (Object[])buff[1])
        {            
            int id = Convert.ToInt32(methodAccess[0]);
            int mode = Convert.ToInt32(methodAccess[1]);
            obj.SetMethodAccess(id, (MethodAccessMode) mode);
        }
    }
    

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            //This should be never called. Server handles this.
            if (index == 2)
            {
                type = DataType.Array;
                return GetObjects();
            }
            if (index == 3)
            {
                type = DataType.None;
                return AssociatedPartnersId;
            }
            if (index == 4)
            {
                type = DataType.None;
                return ApplicationContextName;
            }
            if (index == 5)
            {
                type = DataType.None;
                return XDLMSContextInfo;
            }
            if (index == 6)
            {
                type = DataType.None;
                return AuthenticationMechanismMame;
            }
            if (index == 7)
            {
                type = DataType.None;
                return Secret;
            }
            if (index == 8)
            {
                type = DataType.UInt8;
                return AssociationStatus;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = Convert.ToString(value);                
            }
            else if (index == 2)
            {
                ObjectList.Clear();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        ObjectType type = (ObjectType)Convert.ToInt32(item[0]);
                        int version = Convert.ToInt32(item[1]);
                        String ln = GXDLMSObject.toLogicalName((byte[])item[2]);
                        GXDLMSObject obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                        obj.LogicalName = ln;
                        UpdateAccessRights(obj, (Object[])item[3]);
                        obj.LogicalName = ln;
                        obj.Version = version;
                        ObjectList.Add(obj);
                    }
                }
            }
            else if (index == 3)
            {
                if (value == null)
                {
                    AssociatedPartnersId = null;
                }
                else
                {
                    //TODO: MIKKO AssociatedPartnersId();
                }

            }
            else if (index == 4)
            {
                if (value == null)
                {
                    ApplicationContextName = null;
                }
                else
                {
                    //TODO: MIKKO ApplicationContextName();
                }
            }
            else if (index == 5)
            {
                if (value == null)
                {
                    XDLMSContextInfo = null;
                }
                else
                {
                    //TODO: MIKKO XDLMSContextInfo();
                }
            }
            else if (index == 6)
            {
                if (value == null)
                {
                    AuthenticationMechanismMame = null;
                }
                else
                {
                    //TODO: MIKKO AuthenticationMechanismMame();
                }
            }
            else if (index == 7)
            {
                if (value == null)
                {
                    Secret = null;
                }
                else
                {
                    //TODO: MIKKO Secret();
                }
            }
            else if (index == 8)
            {
                if (value == null)
                {
                    AssociationStatus = AssociationStatus.NonAssociated;
                }
                else
                {
                    //Mikko TODO: AssociationStatus(AssociationStatus.);
                }
            }
            else if (index == 9)
            {
                SecuritySetupReference = (Convert.ToString(value));
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }    
}