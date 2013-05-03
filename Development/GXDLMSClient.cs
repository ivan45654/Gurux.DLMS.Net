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
using System.ComponentModel;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects;
using Gurux.DLMS.ManufacturerSettings;
using System.Reflection;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    public class GXDLMSClient
    {
        public static bool CanRead(AccessMode mode)
        {
            return mode == AccessMode.Read || mode == AccessMode.ReadWrite || mode == AccessMode.AuthenticatedRead || mode == AccessMode.AuthenticatedReadWrite;
        }

        public static bool CanWrite(AccessMode mode)
        {
            return mode == AccessMode.Write || mode == AccessMode.ReadWrite || mode == AccessMode.AuthenticatedWrite || mode == AccessMode.AuthenticatedReadWrite;
        }

        GXDLMS m_Base;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSClient()
        {
            m_Base = new GXDLMS(false);
            this.Authentication = Authentication.None;
        }
        
        /// <summary>
        /// List of available obis codes.
        /// </summary>
        /// <remarks>
        /// This list is used when Association view is read from the meter and description of the object is needed.
        /// If collection is not set description of object is empty.
        /// </remarks>
        public Gurux.DLMS.ManufacturerSettings.GXObisCodeCollection ObisCodes
        {
            get;
            set;
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the sent packet.
        /// </summary>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the sent packet. False, if not.</returns>
        public bool IsReplyPacket(byte[] sendData, byte[] receivedData)
        {
            return m_Base.IsReplyPacket(sendData, receivedData);
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the previous sent packet.
        /// </summary>
        /// <remarks>
        /// In HDLC framing data is sometimes coming late.
        /// </remarks>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the previous sent packet. False, if not.</returns>
        public bool IsPreviousPacket(byte[] sendData, byte[] receivedData)
        {
            return m_Base.IsPreviousPacket(sendData, receivedData);
        }        

        /// <summary>
        /// Returns frame number.
        /// </summary>
        /// <param name="data">Byte array where frame number is try to found.</param>
        /// <returns>Frame number between Zero to seven (0-7).</returns>
        public int GetFrameNumber(byte[] data)
        {
            return m_Base.GetFrameNumber(data);
        }

        /// <summary>
        /// Client ID is the identification of the device that is used as a client.
        /// Client ID is aka HDLC Address.
        /// </summary>
        public object ClientID
        {
            get
            {
                return m_Base.ClientID;
            }
            set
            {
                m_Base.ClientID = value;
            }
        }

        /// <summary>
        /// Server ID is the indentification of the device that is used as a server.
        /// Server ID is aka HDLC Address.
        /// </summary>
        public object ServerID
        {
            get
            {
                return m_Base.ServerID;
            }
            set
            {
                m_Base.ServerID = value;
            }
        }

        /// <summary>
        /// Set server ID.
        /// </summary>
        /// <remarks>
        /// This method is reserved for languages like Python where is no byte size.
        /// </remarks>
        /// <param name="value">Server ID.</param>
        /// <param name="size">Size of server ID as bytes.</param>
        public void SetServerID(object value, int size)
        {
            if (size != 1 && size != 2 && size != 4)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (size == 1)
            {
                m_Base.ServerID = Convert.ToByte(value);
            }
            else if (size == 2)
            {
                m_Base.ServerID = Convert.ToUInt16(value);
            }
            else if (size == 4)
            {
                m_Base.ServerID = Convert.ToUInt32(value);
            }
        }

        /// <summary>
        /// Set client ID.
        /// </summary>
        /// <remarks>
        /// This method is reserved for languages like Python where is no byte size.
        /// </remarks>
        /// <param name="value">Client ID.</param>
        /// <param name="size">Size of server ID as bytes.</param>
        public void SetClientID(object value, int size)
        {
            if (size != 1 && size != 2 && size != 4)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (size == 1)
            {
                m_Base.ClientID = Convert.ToByte(value);
            }
            else if (size == 2)
            {
                m_Base.ClientID = Convert.ToUInt16(value);
            }
            else if (size == 4)
            {
                m_Base.ClientID = Convert.ToUInt32(value);
            }
        }


        /// <summary>
        /// Are BOP, EOP and checksum added to the packet.
        /// </summary>
        public bool GenerateFrame
        {
            get
            {
                return m_Base.GenerateFrame;
            }
            set
            {
                m_Base.GenerateFrame = value;
            }
        }

        /// <summary>
        /// Is cache used. Default value is True;
        /// </summary>
        public bool UseCache
        {
            get
            {
                return m_Base.UseCache;
            }
            set
            {
                m_Base.UseCache = value;
            }
        }

        /// <summary>
        /// DLMS version number. 
        /// </summary>
        /// <remarks>
        /// Gurux DLMS component supports DLMS version number 6.
        /// </remarks>
        /// <seealso cref="SNRMRequest"/>
        [DefaultValue(6)]
        public byte DLMSVersion
        {
            get
            {
                return m_Base.DLMSVersion;
            }
            set
            {
                m_Base.DLMSVersion = value;
            }
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get
            {
                return m_Base.MaxReceivePDUSize;
            }
            set
            {
                m_Base.MaxReceivePDUSize = value;
            }
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.     
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacurer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get
            {
                return m_Base.UseLogicalNameReferencing;
            }
            set
            {
                m_Base.UseLogicalNameReferencing = value;
            }
        }

        /// <summary>
        /// Retrieves the password that is used in communication.
        /// </summary>
        /// <remarks>
        /// If authentication is set to none, password is not used.
        /// </remarks>
        /// <seealso cref="Authentication"/>
        public string Password
        {
            get
            {
                return m_Base.Password;
            }
            set
            {
                m_Base.Password = value;
            }
        }

        /// <summary>
        /// Gets Logical Name settings, read from the device. 
        /// </summary>
        public GXDLMSLNSettings LNSettings
        {
            get
            {
                return m_Base.LNSettings;
            }
        }

        /// <summary>
        /// Gets Short Name settings, read from the device.
        /// </summary>
        public GXDLMSSNSettings SNSettings
        {
            get
            {
                return m_Base.SNSettings;
            }
        }


        /// <summary>
        /// Quality Of Service is an analysis of nonfunctional aspects of the software properties.
        /// </summary>
        /// <returns></returns>
        public int ValueOfQualityOfService
        {
            get
            {
                return m_Base.ValueOfQualityOfService;
            }            
        }

        /// <summary>
        /// Retrieves the amount of unused bits.
        /// </summary>
        /// <returns></returns>
        public int NumberOfUnusedBits
        {
            get
            {
                return m_Base.NumberOfUnusedBits;
            }
        }
       
        /// <summary>
        /// Retrieves the data type. 
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <returns>The current data type.</returns>
        public DataType GetDLMSDataType(byte[] data)
        {
            //Return cache size.
            if (UseCache && data.Length == m_Base.CacheSize)
            {
                return m_Base.CacheType;
            }
            if (!UseCache)
            {
                m_Base.ClearProgress();
            }
            else if (m_Base.CacheIndex != 0)
            {
                return m_Base.CacheType;
            }
            DataType type;
            object value = null;
            m_Base.ParseReplyData(UseCache ? ActionType.Index : ActionType.Count, data, out value, out type);
            return type;
        }


        /// <summary>
        /// Retrieves the authentication used in communicating with the device.
        /// </summary>
        /// <remarks>
        /// By default authentication is not used. If authentication is used,
        /// set the password with the Password property.
        /// </remarks>
        /// <seealso cref="Password"/>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="MaxReceivePDUSize"/>    
        [DefaultValue(Authentication.None)]
        public Authentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Determines the type of the connection
        /// </summary>
        /// <remarks>
        /// All DLMS meters do not support the IEC 62056-47 standard.  
        /// If the device does not support the standard, and the connection is made 
        /// using TCP/IP, set the type to InterfaceType.General.
        /// </remarks>    
        public InterfaceType InterfaceType
        {
            get
            {
                return m_Base.InterfaceType;
            }
            set
            {
                m_Base.InterfaceType = value;
            }
        }

        /// <summary>
        /// Information from the connection size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get
            {
                return m_Base.Limits;
            }            
        }   


        /// <summary>
        /// Generates SNRM request.
        /// </summary>
        /// <remarks>
        /// his method is used to generate send SNRMRequest. 
        /// Before the SNRM request can be generated, at least the following 
        /// properties must be set:
        /// <ul>
        /// <li>ClientID</li>
        /// <li>ServerID</li>    
        /// </ul>
        /// <b>Note! </b>According to IEC 62056-47: when communicating using 
        /// TCP/IP, the SNRM request is not send.
        /// </remarks>
        /// <returns>SNRM request as byte array.</returns>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="ParseUAResponse"/>    
        public byte[] SNRMRequest()
        {
            m_Base.MaxReceivePDUSize = 0xFFFF;
            m_Base.ClearProgress();
            //SNRM reguest is not used in network connections.
            if (this.InterfaceType == InterfaceType.Net)
            {
                return null;
            }
            return m_Base.AddFrame((byte)FrameType.SNRM, false, null, 0, 0);
        }
        
        /// <summary>
        /// Parses UAResponse from byte array.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="data"></param>        
        /// <seealso cref="ParseUAResponse"/>        
        public void ParseUAResponse(byte[] data)
        {
            int index = 0, error;
            byte frame;            
            List<byte> arr = new List<byte>(data);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, true, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (this.InterfaceType != InterfaceType.Net && frame != (byte)FrameType.UA)
            {
                throw new GXDLMSException("Not a UA response :" + frame);
            }
            byte FromatID = arr[index++];
            byte GroupID = arr[index++];
            byte GroupLen = arr[index++];
            object val;
            while (index < arr.Count)
            {
                HDLCInfo id = (HDLCInfo)arr[index++];
                byte len = arr[index++];
                switch (len)
                {
                    case 1:
                        val = (byte)arr[index];
                        break;
                    case 2:
                        val = BitConverter.ToUInt16(GXCommon.Swap(arr, index, len), 0);
                        break;
                    case 4:
                        val = BitConverter.ToUInt32(GXCommon.Swap(arr, index, len), 0);
                        break;
                    default:
                        throw new GXDLMSException("Invalid Exception.");
                }
                index += len;
                switch (id)
                {
                    case HDLCInfo.MaxInfoTX:
                        Limits.MaxInfoTX = val;
                        break;
                    case HDLCInfo.MaxInfoRX:
                        Limits.MaxInfoRX = val;
                        break;
                    case HDLCInfo.WindowSizeTX:
                        Limits.WindowSizeTX = val;
                        break;
                    case HDLCInfo.WindowSizeRX:
                        Limits.WindowSizeRX = val;
                        break;
                    default:
                        throw new GXDLMSException("Invalid UA response.");
                }
            }
        }

        /// <summary>
        /// Generate AARQ request. 
        /// </summary>
        /// <remarks>
        /// Because all meters can't read all data in one packet, 
        /// the packet must be split first, by using SplitDataToPackets method.
        /// </remarks>
        /// <param name="Tags"></param>
        /// <returns>AARQ request as byte array.</returns>
        /// <seealso cref="ParseAAREResponse"/>
        /// <seealso cref="SplitDataToPackets"/>
        /// <seealso cref="IsDLMSPacketComplete"/>
        public byte[][] AARQRequest(GXDLMSTagCollection Tags)
        {
            List<byte> buff = new List<byte>();
            m_Base.CheckInit();
            GXAPDU aarq = new GXAPDU(Tags);
            aarq.UseLN = this.UseLogicalNameReferencing;
            if (this.UseLogicalNameReferencing)
            {
                m_Base.SNSettings = null;
                m_Base.LNSettings = new GXDLMSLNSettings(new byte[] { 0x00, 0x7E, 0x1F });
                aarq.UserInformation.ConformanceBlock = LNSettings.m_ConformanceBlock;
            }
            else
            {
                m_Base.LNSettings = null;
                m_Base.SNSettings = new GXDLMSSNSettings(new byte[] { 0x1C, 0x03, 0x20 });
                aarq.UserInformation.ConformanceBlock = SNSettings.m_ConformanceBlock;
            }
            aarq.SetAuthentication(this.Authentication, Password);
            aarq.UserInformation.DLMSVersioNumber = DLMSVersion;
            aarq.UserInformation.MaxReceivePDUSize = MaxReceivePDUSize;
            aarq.CodeData(buff, this.InterfaceType);
            m_Base.FrameSequence = -1;
            m_Base.ExpectedFrame = -1;
            return m_Base.SplitToBlocks(buff, Command.None);
        }

        /// <summary>
        /// Parses the AARE response.
        /// </summary>
        /// <param name="reply"></param>
        /// <remarks>
        /// Parse method will update the following data:
        /// <ul>
        /// <li>DLMSVersion</li>
        /// <li>MaxReceivePDUSize</li>
        /// <li>UseLogicalNameReferencing</li>
        /// <li>LNSettings or SNSettings</li>
        /// </ul>
        /// LNSettings or SNSettings will be updated, depending on the referencing, 
        /// Logical name or Short name.
        /// </remarks>
        /// <returns>The AARE response</returns>
        /// <seealso cref="AARQRequest"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        /// <seealso cref="LNSettings"/>
        /// <seealso cref="SNSettings"/>
        public GXDLMSTagCollection ParseAAREResponse(byte[] reply)
        {
            byte frame;
            int error, index = 0;            
            List<byte> arr = new List<byte>(reply);
            bool packetFull, wrongCrc;
            byte command;
            m_Base.GetDataFromFrame(arr, index, out frame, true, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            //Parse AARE data.
            GXDLMSTagCollection Tags = new GXDLMSTagCollection();
            GXAPDU pdu = new GXAPDU(Tags);
            pdu.EncodeData(arr.ToArray(), ref index);
            UseLogicalNameReferencing = pdu.UseLN;
            if (UseLogicalNameReferencing)
            {
                System.Diagnostics.Debug.WriteLine("--- Logical Name settings are---\r\n");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- Short Name settings are---\r\n");
            }
            AssociationResult ret = pdu.ResultComponent;
            if (ret == AssociationResult.Accepted)
            {
                System.Diagnostics.Debug.WriteLine("- Client has accepted connection.");
                if (UseLogicalNameReferencing)
                {
                    m_Base.LNSettings = new GXDLMSLNSettings(pdu.UserInformation.ConformanceBlock);
                }
                else
                {
                    m_Base.SNSettings = new GXDLMSSNSettings(pdu.UserInformation.ConformanceBlock);
                }
                MaxReceivePDUSize = pdu.UserInformation.MaxReceivePDUSize;
                DLMSVersion = pdu.UserInformation.DLMSVersioNumber;
            }
            else
            {
                throw new GXDLMSException(ret, pdu.ResultDiagnosticValue);
            }
            System.Diagnostics.Debug.WriteLine("- Server max PDU size is " + MaxReceivePDUSize);
            System.Diagnostics.Debug.WriteLine("- Value of quality of service is " + ValueOfQualityOfService);
            System.Diagnostics.Debug.WriteLine("- Server DLMS version number is " + DLMSVersion);
            if (DLMSVersion != 6)
            {
                throw new GXDLMSException("Invalid DLMS version number.");
            }
            System.Diagnostics.Debug.WriteLine("- Number of unused bits is " + NumberOfUnusedBits);
            return Tags;
        }

        /// <summary>
        /// Generates a disconnect mode request.
        /// </summary>
        /// <returns>Disconnect mode request, as byte array.</returns>
        public byte[] DisconnectedModeRequest()
        {
            m_Base.ClearProgress();
            //If connection is not established, there is no need to send DisconnectRequest.
            if (SNSettings == null && LNSettings == null) //TODO:
            {
                return null;
            }
            //In current behavior, disconnect is not generated for network connection.
            if (this.InterfaceType != InterfaceType.Net)
            {
                return m_Base.AddFrame((byte)FrameType.DisconnectMode, false, null, 0, 0);
            }
            return null;
        }

        /// <summary>
        /// Generates a disconnect request.
        /// </summary>
        /// <returns>Disconnected request, as byte array.</returns>
        public byte[] DisconnectRequest()
        {
            m_Base.ClearProgress();
            //If connection is not established there is no need to send DisconnectRequest.
            if (SNSettings == null && LNSettings == null)
            {
                return null;
            }
            if (this.InterfaceType != InterfaceType.Net)
            {
                return m_Base.AddFrame((byte)FrameType.Disconnect, false, null, 0, 0);
            }
            List<byte> data = new List<byte>(new byte[] { 0x62, 0x0 });
            return m_Base.AddFrame((byte)FrameType.Disconnect, false, data, 0, data.Count);
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serizlization.
        /// </remarks>
        public static Type[] GetObjectTypes()
        {
            return GXDLMS.GetObjectTypes();
        }        

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="ClassID"></param>
        /// <param name="Version"></param>
        /// <param name="BaseName"></param>
        /// <param name="LN"></param>
        /// <param name="AccessRights"></param>
        /// <param name="AttributeIndex"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        GXDLMSObject CreateDLMSObject(int ClassID, object Version, int BaseName, object LN, object AccessRights, int AttributeIndex, int dataIndex)
        {
            GXDLMSObject obj = null;
            ObjectType type = (ObjectType)ClassID;
            if (GXDLMS.AvailableObjectTypes.ContainsKey(type))
            {
                Type tmp = GXDLMS.AvailableObjectTypes[type];
                obj = Activator.CreateInstance(tmp) as GXDLMSObject;
            }
            else
            {
                obj = new GXDLMSObject();
            }
            UpdateObjectData(obj, type, Version, BaseName, (byte[])LN, AccessRights, AttributeIndex, dataIndex);
            if (ObisCodes != null)
            {
                Gurux.DLMS.ManufacturerSettings.GXObisCode code = ObisCodes.FindByLN(obj.ObjectType, obj.LogicalName, null);
                if (code != null)
                {
                    obj.Description = code.Description;
                    obj.Attributes.AddRange(code.Attributes);
                }
            }
            return obj;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseSNObjects(byte[] buff, bool onlyKnownObjects)
        {
            int index = 0;
            //Get array tag.
            byte size = buff[index++];
            //Check that data is in the array
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            long cnt = GXCommon.GetObjectCount(buff, ref index);
            int total, count;
            int[] values = null;
            if (onlyKnownObjects)
            {
                Array arr = Enum.GetValues(typeof(ObjectType));
                values = new int[arr.Length];
                arr.CopyTo(values, 0);
            }
            for (long objPos = 0; objPos != cnt; ++objPos)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(buff, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                int type2 = Convert.ToInt16(objects[1]);
                if (!onlyKnownObjects || values.Contains(type2))
                {
                    int baseName = Convert.ToInt32(objects[0]) & 0xFFFF;
                    if (baseName > 0)
                    {
                        GXDLMSObject comp = CreateDLMSObject(type2, objects[2], baseName, objects[3], null, 0, 0);
                        if (comp != null)
                        {
                            items.Add(comp);
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", type2, objects[0]));
                }
            }
            return items;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="version"></param>
        /// <param name="baseName"></param>
        /// <param name="logicalName"></param>
        /// <param name="accessRights"></param>
        /// <param name="attributeIndex"></param>
        /// <param name="dataIndex"></param>
        internal static void UpdateObjectData(GXDLMSObject obj, ObjectType objectType, object version, object baseName, byte[] logicalName, object accessRights, int attributeIndex, int dataIndex)
        {
            obj.ObjectType = objectType;
            // Check access rights...            
            if (accessRights != null && accessRights.GetType().IsArray)
            {
                //access_rights: access_right
                object[] access = (object[])accessRights;
                foreach (object[] attributeAccess in (object[])access[0])
                {
                    int id = Convert.ToInt32(attributeAccess[0]);
                    AccessMode mode = (AccessMode)Convert.ToInt32(attributeAccess[1]);
                    obj.SetAccess(id, mode);
                }
                if (obj.ShortName == 0) //If Logical Name is used.
                {
                }
                else //If Short Name is used.
                {
                    foreach (object[] methodAccess in (object[])access[1])
                    {
                        int id = Convert.ToInt32(methodAccess[0]);
                        MethodAccessMode mode = (MethodAccessMode)Convert.ToInt32(methodAccess[1]);
                        obj.SetMethodAccess(id, mode);
                    }
                }
            }
            ((IGXDLMSColumnObject)obj).SelectedAttributeIndex = attributeIndex;
            ((IGXDLMSColumnObject)obj).SelectedDataIndex = dataIndex;
            if (baseName != null)
            {
                obj.ShortName = Convert.ToUInt16(baseName);
            }
            if (version != null)
            {
                obj.Version = Convert.ToInt32(version);
            }
            obj.LogicalName = GXDLMSObject.toLogicalName(logicalName);
        }

        /// <summary>
        /// Parses the COSEM objects of the received data.
        /// </summary>
        /// <param name="data">Received data, from the device, as byte array. </param>
        /// <returns>Collection of COSEM objects.</returns>
        public GXDLMSObjectCollection ParseObjects(byte[] data, bool onlyKnownObjects)
        {
            if (data == null || data.Length == 0)
            {
                throw new GXDLMSException("ParseObjects failed. Invalid parameter.");
            }
            GXDLMSObjectCollection objects = null;
            if (UseLogicalNameReferencing)
            {
                objects = ParseLNObjects(data, onlyKnownObjects);
            }
            else
            {
                objects = ParseSNObjects(data, onlyKnownObjects);
            }
            return objects;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        GXDLMSObjectCollection ParseLNObjects(byte[] buff, bool onlyKnownObjects)
        {
            int index = 0;
            byte size = buff[index++];
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(buff, ref index);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            int total, count;
            int[] values = null;
            if (onlyKnownObjects)
            {
                Array arr = Enum.GetValues(typeof(ObjectType));
                values = new int[arr.Length];
                arr.CopyTo(values, 0);
            }
            //Some meters give wrong item count.
            while (index != buff.Length && cnt != objectCnt)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(buff, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                int type2 = Convert.ToInt16(objects[0]);
                if (!onlyKnownObjects || values.Contains(type2))
                {
                    GXDLMSObject comp = CreateDLMSObject(type2, objects[1], 0, objects[2], objects[3], 0, 0);
                    items.Add(comp);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Unknown object : {0} {1}", type2, objects[2]));
                }
            }
            return items;
        }

        /// <summary>
        /// Parse data columns.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public GXDLMSObjectCollection ParseColumns(byte[] data)
        {
            if (data == null)
            {
                throw new GXDLMSException("Invalid parameter.");
            }
            int index = 0;
            byte size = data[index++];
            //Check that data is in the array.
            if (size != 0x01)
            {
                throw new GXDLMSException("Invalid response.");
            }
            //get object count
            int cnt = GXCommon.GetObjectCount(data, ref index);
            int objectCnt = 0;
            GXDLMSObjectCollection items = new GXDLMSObjectCollection();
            int total, count;
            while (index != data.Length && cnt != objectCnt)
            {
                DataType type = DataType.None;
                int cachePosition = 0;
                object[] objects = (object[])GXCommon.GetData(data, ref index, ActionType.None, out total, out count, ref type, ref cachePosition);
                if (index == -1)
                {
                    throw new OutOfMemoryException();
                }
                if (objects.Length != 4)
                {
                    throw new GXDLMSException("Invalid structure format.");
                }
                ++objectCnt;
                GXDLMSObject comp = CreateDLMSObject(Convert.ToInt16(objects[0]), null, 0, objects[1], 0, Convert.ToInt16(objects[2]), Convert.ToInt16(objects[3]));
                if (comp != null)
                {
                    items.Add(comp);
                }
            }
            return items;
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        public object UpdateValue(byte[] data, GXDLMSObject target, int attributeIndex)
        {
            target.SetValue(attributeIndex, GetValue(data, target, attributeIndex));
            DataType type;
            return (target as IGXDLMSBase).GetValue(attributeIndex, out type, null); //Mikko TODO:
        }

        /*
         * Get Value from byte array received from the meter.
        */
        public object GetValue(byte[] data, GXDLMSObject target, int attributeIndex)
        {        
            DataType type = target.GetDataType(attributeIndex);
            Object value = GetValue(data);
            if (value is byte[] && type != DataType.None)
            {
                return GXDLMSClient.ChangeType((byte[]) value, type);
            }
            return value;
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="ln"></param>
        /// <param name="attributeIndex"></param>
        /// <returns></returns>
        public object GetValue(byte[] data, ObjectType type, string ln, int attributeIndex)
        {
            object value = GetValue(data);
            //If logical name.
            if (attributeIndex == 1)
            {
                return ChangeType((byte[])value, DataType.OctetString);
            }
            if (value is byte[] && ObisCodes != null)
            {
                Gurux.DLMS.ManufacturerSettings.GXObisCode code = ObisCodes.FindByLN(type, ln, null);
                if (code != null)
                {
                    GXDLMSAttributeSettings att = code.Attributes.Find(attributeIndex);
                    if (att != null && value != null && ((byte[])value).Length != 0)
                    {
                        return ChangeType((byte[])value, att.UIType);
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Get Value from byte array received from the meter.
        /// </summary>
        /// <param name="data">Byte array received from the meter.</param>
        /// <param name="rawData"></param>
        /// <returns>Received data.</returns>
        public object GetValue(byte[] data)
        {
            if (!UseCache || data.Length < m_Base.CacheIndex)
            {
                m_Base.ClearProgress();
            }
            //Return cached items.
            if (UseCache && m_Base.CacheSize == data.Length)
            {
                return m_Base.CacheData;
            }
            DataType type;
            object value = null;
            m_Base.ParseReplyData(UseCache ? ActionType.Index : ActionType.None, data, out value, out type);
            return m_Base.CacheData;
        }

        /// <summary>
        /// TryGetValue try parse multirow value from byte array to variant.
        /// </summary>
        /// <remarks>
        /// This method can be used when Profile Generic is read and if 
        /// data is need to update at collection time.
        /// Cached data is cleared after read.
        /// </remarks>
        /// <param name="data">Byte array received from the meter.</param>        
        /// <returns>Received data.</returns>
        /// <seealso cref="UseCache">UseCache</seealso>
        public object TryGetValue(byte[] data)
        {
            if (!UseCache || data.Length < m_Base.CacheIndex)
            {
                m_Base.ClearProgress();
            }
            DataType type = DataType.None;
            int read, total, index = 0;
            try
            {
                //Return cached items.
                if (UseCache)
                {
                    if (m_Base.CacheSize == data.Length)
                    {
                        //Clear cached data after read.
                        object tmp = m_Base.CacheData;
                        m_Base.CacheData = null;
                        return tmp;
                    }
                    if (m_Base.CacheData != null)
                    {
                        throw new Exception("Cache data is not empty.");
                    }
                }
                object value = GXCommon.GetData(data, ref index, ActionType.None, out total, out read, ref type, ref m_Base.CacheIndex);
                if (UseCache)
                {
                    m_Base.CacheData = null;
                    m_Base.ItemCount += read;
                    m_Base.CacheSize = data.Length;
                    m_Base.MaxItemCount += total;
                }
                return value;
            }
            catch
            {
                return null;
            }
        }

        public static GXDLMSAttributeSettings GetAttributeInfo(GXDLMSObject item, int index)
        {
            GXDLMSAttributeSettings att = item.Attributes.Find(index);
            if (att != null)
            {
                return att;
            }
            GXDLMSAttribute att2;
            PropertyInfo info;
            GetPropertyInfo(item, index, out att2, out info);
            return att2;
        }

        public static PropertyInfo GetPropertyInfo(object item, int index, out GXDLMSAttribute att, out PropertyInfo info)
        {
            info = null;
            att = null;
            PropertyInfo[] fields = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => Attribute.IsDefined(x, typeof(GXDLMSAttribute), false)).ToArray();
            foreach (PropertyInfo it in fields)
            {
                GXDLMSAttribute tmp = Attribute.GetCustomAttribute(it, typeof(GXDLMSAttribute)) as GXDLMSAttribute;
                if (tmp.Index == index)
                {
                    info = it;
                    att = tmp;
                    return it;
                }
            }
            return null;
        }

        /// <summary>
        /// Changes byte array received from the meter to given type.
        /// </summary>
        /// <param name="value">Byte array received from the meter.</param>
        /// <param name="type">Wanted type.</param>
        /// <returns>Value changed by type.</returns>
        public static object ChangeType(byte[] value, DataType type)
        {
            if (value == null)
            {
                return null;
            }
            if (type == DataType.None)
            {
                return BitConverter.ToString(value).Replace('-', ' ');
            }
            int index = 0;
            int read, total, cachePosition = 0;
            object ret = GXCommon.GetData(value, ref index, ActionType.None, out total, out read, ref type, ref cachePosition);
            if (index == -1)
            {
                throw new OutOfMemoryException();
            }
            if (type == DataType.OctetString && ret is byte[])
            {
                string str = null;
                byte[] arr = (byte[])ret;
                if (arr.Length == 0)
                {
                    str = string.Empty;
                }
                else
                {
                    foreach (byte it in arr)
                    {
                        if (str != null)
                        {
                            str += ".";
                        }
                        str += it.ToString();
                    }
                }
                return str;
            }
            return ret;
        }

         /// <summary>
        /// Reads the selected object from the device.
        /// </summary>
        /// <remarks>
        /// This method is used to get all registers in the device.
        /// </remarks>
        /// <returns>Read request, as byte array.</returns>
        public byte[] GetObjects()
        {
            object name;
            if (UseLogicalNameReferencing)
            {
                name = "0.0.40.0.0.255";
            }
            else
            {
                name = (ushort)0xFA00;
            }
            return Read(name, ObjectType.AssociationLogicalName, 2)[0];
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[] Method(GXDLMSObject item, int index, Object data)
        {
            return Method(item.Name, item.ObjectType, index, data, DataType.None);
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[] Method(GXDLMSObject item, int index, Object data, DataType type)
        {
            return Method(item.Name, item.ObjectType, index, data, type);
        }

        /// <summary>
        /// Generate Method (Action) request.
        /// </summary>
        /// <param name="name">Method object short name or Logical Name.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="index">Methdod index.</param>
        /// <returns></returns>
        public byte[] Method(object name, ObjectType objectType, int index, Object data, DataType type)
        {
            if (name == null || index < 1)
            {
                throw new ArgumentOutOfRangeException("Invalid parameter");
            }
            m_Base.ClearProgress();
            if (type == DataType.None)
            {
                type = GXCommon.GetValueType(data);
            }
            List<byte> buff = new List<byte>();
            GXCommon.SetData(buff, type, data);
            if (!this.UseLogicalNameReferencing)
            {
                int value, count;
                GXDLMS.GetActionInfo(objectType, out value, out count);
                if (index > count)
                {
                    throw new ArgumentOutOfRangeException("methodIndex");
                }
                index = (value + (index - 1) * 0x8);
            }
            return m_Base.GenerateMessage(name, 0, buff.ToArray(), objectType, index, Command.MethodRequest)[0];
        }


        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="item">object to write.</param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Write(GXDLMSObject item, int index)
        {
            if (item == null || index < 1)
            {
                throw new GXDLMSException("Invalid parameter");
            }
            object value = null;
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
            bool found = false;
            DataType type = DataType.None;
            foreach (PropertyDescriptor it in pdc)
            {
                GXDLMSAttribute att = it.Attributes[typeof(GXDLMSAttribute)] as GXDLMSAttribute;
                if (att != null && att.Index == index)
                {
                    found = true;
                    value = it.GetValue(item);
                    type = item.GetDataType(index);
                    break;
                }
            }
            if (!found)
            {
                throw new Exception("Index out of range.");
            }            
            return Write(item.Name, value, type, item.ObjectType, index);
        }

        /// <summary>
        /// Generates a write message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="value">Data to Write.</param>
        /// <param name="type">Data type of write object.</param>
        /// <param name="objectType"></param>
        /// <param name="index">Attribute index where data is write.</param>
        /// <returns></returns>
        public byte[][] Write(object name, object value, DataType type, ObjectType objectType, int index)
        {
            if (index < 1)
            {
                throw new GXDLMSException("Invalid parameter");
            }
            if (type == DataType.None)
            {
                type = GXCommon.GetValueType(value);
            }
            m_Base.ClearProgress();
            List<byte> data = new List<byte>();
            GXCommon.SetData(data, type, value);            
            return m_Base.GenerateMessage(name, 2, data.ToArray(), objectType, index, UseLogicalNameReferencing ? Command.SetRequest : Command.WriteRequest);
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="name">Short or Logical Name.</param>
        /// <param name="objectType">Read Interface.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(object name, ObjectType objectType, int attributeOrdinal)
        {
            if ((attributeOrdinal < 0))
            {
                throw new GXDLMSException("Invalid parameter");
            }
            //Clear cache
            m_Base.ClearProgress();
            return m_Base.GenerateMessage(name, 2, new byte[0], objectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest);
        }

        /// <summary>
        /// Generates a read message.
        /// </summary>
        /// <param name="item">DLMS object to read.</param>
        /// <param name="attributeOrdinal">Read attribute index.</param>
        /// <returns>Read request as byte array.</returns>
        public byte[][] Read(GXDLMSObject item, int attributeOrdinal)
        {
            if ((attributeOrdinal < 1))
            {
                throw new GXDLMSException("Invalid parameter");
            }
            //Clear cache
            m_Base.ClearProgress();
            return m_Base.GenerateMessage(item.Name, 2, new byte[0], item.ObjectType, attributeOrdinal, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest);
        }

        /// <summary>
        /// Generates the keep alive message. 
        /// </summary>
        /// <remarks>
        /// Keep alive message is sent to keep the connection to the device alive.
        /// </remarks>
        /// <returns>Returns Keep alive message, as byte array.</returns>
        public byte[] GetKeepAlive()
        {
            m_Base.ClearProgress();
            //There is no keepalive in IEC 62056-47.
            if (this.InterfaceType == InterfaceType.Net)
            {
                return null;
            }
            return m_Base.AddFrame(m_Base.GenerateAliveFrame(), false, null, 0, 0);
        }

        /// <summary>
        /// Read rows by entry.
        /// </summary>
        /// <param name="name">object name.</param>
        /// <param name="index">Zero bases start index.</param>
        /// <param name="count">Rows count to read.</param>
        /// <returns>Read message as byte array.</returns>
        public byte[] ReadRowsByEntry(object name, int index, int count)
        {
            m_Base.ClearProgress();
            List<byte> buff = new List<byte>();
            buff.Add(0x02);  //Add AccessSelector
            buff.Add((byte)DataType.Structure); //Add enum tag.
            buff.Add(0x04); //Add item count
            GXCommon.SetData(buff, DataType.UInt32, index); //Add start index
            GXCommon.SetData(buff, DataType.UInt32, count);//Add Count
            GXCommon.SetData(buff, DataType.UInt16, 0);//Read all columns.
            GXCommon.SetData(buff, DataType.UInt16, 0);
            return m_Base.GenerateMessage(name, 4, buff.ToArray(), ObjectType.ProfileGeneric, 2, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest)[0];
        }

        /// <summary>
        /// Read rows by range.
        /// </summary>
        /// <remarks>
        /// Use this method to read Profile Generic table between dates.
        /// </remarks>
        /// <param name="name">object name.</param>
        /// <param name="ln">The logical name of the first column.</param>
        /// <param name="objectType">The ObjectType of the first column.</param>
        /// <param name="version">The version of the first column.</param>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        /// <returns></returns>
        public byte[] ReadRowsByRange(object name, string ln, ObjectType objectType, int version, DateTime start, DateTime end)
        {
            m_Base.ClearProgress();
            List<byte> buff = new List<byte>();
            buff.Add(0x01);  //Add AccessSelector
            buff.Add((byte)DataType.Structure); //Add enum tag.
            buff.Add(0x04); //Add item count
            buff.Add(0x02); //Add enum tag.
            buff.Add(0x04); //Add item count           
            GXCommon.SetData(buff, DataType.UInt16, (ushort)8);// Add class_id	            
            GXCommon.SetData(buff, DataType.OctetString, ln);// Add parameter Logical name
            GXCommon.SetData(buff, DataType.Int8, 2);// Add attribute index.
            GXCommon.SetData(buff, DataType.UInt16, version);// Add version
            GXCommon.SetData(buff, DataType.DateTime, start);// Add start time.
            GXCommon.SetData(buff, DataType.DateTime, end);// Add end time.
            //Add array of read columns. Read All...
            buff.Add(0x01); //Add item count   
            buff.Add(0x00); //Add item count   
            return m_Base.GenerateMessage(name, 4, buff.ToArray(), ObjectType.ProfileGeneric, 2, this.UseLogicalNameReferencing ? Command.GetRequest : Command.ReadRequest)[0];
        }

        public static GXDLMSObject CreateObject(Gurux.DLMS.ObjectType type)
        {
            return GXDLMS.CreateObject(type);
        }        

        /// <summary>
        /// Determines, whether the DLMS packet is completed.
        /// </summary>
        /// <param name="data">The data to be parsed, to complete the DLMS packet.</param>
        /// <returns>True, when the DLMS packet is completed.</returns>
        public bool IsDLMSPacketComplete(byte[] data)
        {
            return m_Base.IsDLMSPacketComplete(data);
        }

        public object[,] CheckReplyErrors(byte[] sendData, byte[] receivedData)
        {
            return m_Base.CheckReplyErrors(sendData, receivedData);
        }

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to 
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        public byte[] ReceiverReady(RequestTypes type)
        {
            return m_Base.ReceiverReady(type);
        }

        /// <summary>
        /// This method is used to solve current index of received DLMS packet, 
        /// by retrieving the current progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>The current index of the packet.</returns>
        public int GetCurrentProgressStatus(byte[] data)
        {
            return m_Base.GetCurrentProgressStatus(data);
        }

        /// <summary>
        /// This method is used to solve the total amount of received items,
        /// by retrieving the maximum progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>Total amount of received items.</returns>
        public int GetMaxProgressStatus(byte[] data)
        {
            return m_Base.GetMaxProgressStatus(data);
        }

        /// <summary>
        /// Removes the HDLC header from the packet, and returns COSEM data only.
        /// </summary>
        /// <param name="packet">The received packet, from the device, as byte array.</param>
        /// <param name="data">The exported data.</param>
        /// <returns>COSEM data.</returns>
        public RequestTypes GetDataFromPacket(byte[] packet, ref byte[] data)
        {
            byte frame;
            byte command;
            return m_Base.GetDataFromPacket(packet, ref data, out frame, out command);
        }

        /// <summary>
        /// Returns unit text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUnit(Unit value)
        {
            switch (value)
            {
                case Unit.Year:
                    return Gurux.DLMS.Properties.Resources.UnitYearTxt;
                case Unit.Month:
                    return Gurux.DLMS.Properties.Resources.UnitMonthTxt;
                case Unit.Week:
                    return Gurux.DLMS.Properties.Resources.UnitWeekTxt;
                case Unit.Day:
                    return Gurux.DLMS.Properties.Resources.UnitDayTxt;
                case Unit.Hour:
                    return Gurux.DLMS.Properties.Resources.UnitHourTxt;
                case Unit.Minute:
                    return Gurux.DLMS.Properties.Resources.UnitMinuteTxt;
                case Unit.Second:
                    return Gurux.DLMS.Properties.Resources.UnitSecondTxt;
                case Unit.PhaseAngleGegree:
                    return Gurux.DLMS.Properties.Resources.UnitPhasAngleGegreeTxt;
                case Unit.Temperature:
                    return Gurux.DLMS.Properties.Resources.UnitTemperatureTxt;
                case Unit.LocalCurrency:
                    return Gurux.DLMS.Properties.Resources.UnitLocalCurrencyTxt;
                case Unit.Length:
                    return Gurux.DLMS.Properties.Resources.UnitLengthTxt;
                case Unit.Speed:
                    return Gurux.DLMS.Properties.Resources.UnitSpeedTxt;
                case Unit.VolumeCubicMeter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeCubicMeterTxt;
                case Unit.CorrectedVolume:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeTxt;
                case Unit.VolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxHourTxt;
                case Unit.CorrectedVolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeFluxHourTxt;
                case Unit.VolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxDayTxt;
                case Unit.CorrecteVolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitCorrecteVolumeFluxDayTxt;
                case Unit.VolumeLiter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeLiterTxt;
                case Unit.MassKg:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgTxt;
                case Unit.Force:
                    return Gurux.DLMS.Properties.Resources.UnitForceTxt;
                case Unit.Energy:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyTxt;
                case Unit.PressurePascal:
                    return Gurux.DLMS.Properties.Resources.UnitPressurePascalTxt;
                case Unit.PressureBar:
                    return Gurux.DLMS.Properties.Resources.UnitPressureBarTxt;
                case Unit.EnergyJoule:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyJouleTxt;
                case Unit.ThermalPower:
                    return Gurux.DLMS.Properties.Resources.UnitThermalPowerTxt;
                case Unit.ActivePower:
                    return Gurux.DLMS.Properties.Resources.UnitActivePowerTxt;
                case Unit.ApparentPower:
                    return Gurux.DLMS.Properties.Resources.UnitApparentPowerTxt;
                case Unit.ReactivePower:
                    return Gurux.DLMS.Properties.Resources.UnitReactivePowerTxt;
                case Unit.ActiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitActiveEnergyTxt;
                case Unit.ApparentEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitApparentEnergyTxt;
                case Unit.ReactiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveEnergyTxt;
                case Unit.Current:
                    return Gurux.DLMS.Properties.Resources.UnitCurrentTxt;
                case Unit.ElectricalCharge:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalChargeTxt;
                case Unit.Voltage:
                    return Gurux.DLMS.Properties.Resources.UnitVoltageTxt;
                case Unit.ElectricalFieldStrength:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalFieldStrengthTxt;
                case Unit.Capacity:
                    return Gurux.DLMS.Properties.Resources.UnitCapacityTxt;
                case Unit.Resistance:
                    return Gurux.DLMS.Properties.Resources.UnitResistanceTxt;
                case Unit.Resistivity:
                    return Gurux.DLMS.Properties.Resources.UnitResistivityTxt;
                case Unit.MagneticFlux:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticFluxTxt;
                case Unit.Induction:
                    return Gurux.DLMS.Properties.Resources.UnitInductionTxt;
                case Unit.Magnetic:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticTxt;
                case Unit.Inductivity:
                    return Gurux.DLMS.Properties.Resources.UnitInductivityTxt;
                case Unit.Frequency:
                    return Gurux.DLMS.Properties.Resources.UnitFrequencyTxt;
                case Unit.Active:
                    return Gurux.DLMS.Properties.Resources.UnitActiveTxt;
                case Unit.Reactive:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveTxt;
                case Unit.Apparent:
                    return Gurux.DLMS.Properties.Resources.UnitApparentTxt;
                case Unit.V260:
                    return Gurux.DLMS.Properties.Resources.UnitV260Txt;
                case Unit.A260:
                    return Gurux.DLMS.Properties.Resources.UnitA260Txt;
                case Unit.MassKgPerSecond:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgPerSecondTxt;
                case Unit.Conductance:
                    return Gurux.DLMS.Properties.Resources.UnitConductanceTxt;
                case Unit.OtherUnit:
                    return Gurux.DLMS.Properties.Resources.UnitOtherTxt;
                case Unit.NoUnit:
                    return Gurux.DLMS.Properties.Resources.UnitNoneTxt;
            }
            return "";
        }       
    }
}