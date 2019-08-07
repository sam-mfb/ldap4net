using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LdapForNet
{
    public abstract class DirectoryRequest
    {
        internal DirectoryRequest(){}
        
        public List<DirectoryControl> Controls { get; } = new List<DirectoryControl>();       
    }
    
    public class DeleteRequest : DirectoryRequest
    {
        public DeleteRequest(string distinguishedName)
        {
            DistinguishedName = distinguishedName;
        }

        public string DistinguishedName { get; set; }
    }

    public class AddRequest : DirectoryRequest
    {
        public AddRequest(LdapEntry ldapEntry)
        {
            LdapEntry = ldapEntry;
        }

        public LdapEntry LdapEntry { get; set; }
    }

    public class ModifyRequest : DirectoryRequest
    {
        public ModifyRequest(LdapModifyEntry ldapModifyEntry)
        {
            LdapEntry = ldapModifyEntry;
        }
        public LdapModifyEntry LdapEntry { get; set; }
    }


    public class ModifyDNRequest : DirectoryRequest
    {
        public ModifyDNRequest(string distinguishedName, string newParentDistinguishedName, string newName)
        {
            DistinguishedName = distinguishedName;
            NewParentDistinguishedName = newParentDistinguishedName;
            NewName = newName;
        }

        public string DistinguishedName { get; set; }

        public string NewParentDistinguishedName { get; set; }

        public string NewName { get; set; }

        public bool DeleteOldRdn { get; set; } = true;
    }

    public class SearchRequest : DirectoryRequest
    {

        private string _directoryFilter = null;
        private Native.Native.LdapSearchScope _directoryScope = Native.Native.LdapSearchScope.LDAP_SCOPE_SUBTREE;
        private int _directorySizeLimit = 0;
        private TimeSpan _directoryTimeLimit = new TimeSpan(0);

        
        public SearchRequest(string distinguishedName, string ldapFilter, Native.Native.LdapSearchScope searchScope,params string[] attributeList)
        {
            DistinguishedName = distinguishedName;
            Scope = searchScope;
            Filter = ldapFilter;
            if (attributeList != null)
            {
                Attributes.AddRange(attributeList);
            }
        }

        public string DistinguishedName { get; set; }


        public string Filter
        {
            get => _directoryFilter;
            set => _directoryFilter = value;
        }

        public Native.Native.LdapSearchScope Scope
        {
            get => _directoryScope;
            set
            {
                if (value < Native.Native.LdapSearchScope.LDAP_SCOPE_BASE || value > Native.Native.LdapSearchScope.LDAP_SCOPE_SUBTREE)
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Native.Native.LdapSearchScope));
                }

                _directoryScope = value;
            }
        }

        public List<string> Attributes { get; } = new List<string>();
        

        public int SizeLimit
        {
            get => _directorySizeLimit;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(nameof(SizeLimit) + " could not negative number", nameof(value));
                }

                _directorySizeLimit = value;
            }
        }

        public TimeSpan TimeLimit
        {
            get => _directoryTimeLimit;
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentException(nameof(TimeLimit) + " could not negative number", nameof(value));
                }

                // Prevent integer overflow.
                if (value.TotalSeconds > int.MaxValue)
                {
                    throw new ArgumentException("Time span overflow", nameof(value));
                }

                _directoryTimeLimit = value;
            }
        }

        public bool AttributesOnly { get; set; }
        
        
    }
    
    public class ExtendedRequest : DirectoryRequest
    {
        private byte[] _requestValue = null;

        public ExtendedRequest() { }

        public ExtendedRequest(string requestName)
        {
            RequestName = requestName;
        }

        public ExtendedRequest(string requestName, byte[] requestValue) : this(requestName)
        {
            _requestValue = requestValue;
        }

        public string RequestName { get; set; }

        public byte[] RequestValue
        {
            get
            {
                if (_requestValue == null)
                {
                    return Array.Empty<byte>();
                }

                byte[] tempValue = new byte[_requestValue.Length];
                for (int i = 0; i < _requestValue.Length; i++)
                {
                    tempValue[i] = _requestValue[i];
                }
                return tempValue;
            }
            set => _requestValue = value;
        }
    }
    
    public class CompareRequest:DirectoryRequest 
    {
        public CompareRequest(LdapEntry ldapEntry)
        {
            LdapEntry = ldapEntry;
        }
        
        public LdapEntry LdapEntry { get; set; }
    }   
}