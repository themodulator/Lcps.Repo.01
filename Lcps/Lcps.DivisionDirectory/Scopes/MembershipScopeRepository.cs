using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using Lcps.DivisionDirectory.Members;

namespace Lcps.DivisionDirectory.Scopes
{
    public class MembershipScopeRepository : GenericRepository<MembershipScope>
    {
        #region Fields

        Lcps.Infrastructure.LcpsRepositoryContext  _context = new Infrastructure.LcpsRepositoryContext();
        Type _membershipScopeType;

        #endregion

        #region Constructors

        public MembershipScopeRepository()
            : base(new Lcps.Infrastructure.LcpsRepositoryContext())
        {
        }

        #endregion

        #region Create

        public long DynamicScopeMinValue()
        {
            var e = Enum.GetValues(typeof(MembershipScopeReserved)).Cast<MembershipScopeReserved>().Last();

            long initValue = ((long)e * 2);

            return initValue;
        }

        public override void Insert(MembershipScope entity)
        {
            if (entity.BinaryValue == 0)
                entity.BinaryValue = FindFirstAvailableBinaryValue();

            base.Insert(entity);
        }

        private long FindFirstAvailableBinaryValue()
        {
            long initValue = DynamicScopeMinValue();

            MembershipScope s = new MembershipScope();
            while (s != null)
            {
                s = this.First(x => x.BinaryValue == initValue);
                if (s != null)
                    initValue = initValue * 2;

            }

            return initValue;
        }

        #endregion

        #region Delete

        public void Delete(long binaryValue)
        {
            MembershipScope s = First(x => x.BinaryValue.Equals(binaryValue));
            if (s != null)
                Delete(s);
        }


        public override void Delete(MembershipScope entityToDelete)
        {
            // Delete the value from all tables that filter on binaryValue

            GenericRepository<DirectoryMember> memberContext = new GenericRepository<DirectoryMember>(Context);



            foreach (DirectoryMember m in memberContext.Get())
            {
                if (HasFlag(m.MembershipScope, entityToDelete.BinaryValue, ScopeEnumType))
                {
                    m.MembershipScope -= entityToDelete.BinaryValue;
                    memberContext.Update(m);
                }
            }

            base.Delete(entityToDelete);
        }

        #endregion

        #region Get

        public override IEnumerable<MembershipScope> Get(System.Linq.Expressions.Expression<Func<MembershipScope, bool>> filter = null, Func<IQueryable<MembershipScope>, IOrderedQueryable<MembershipScope>> orderBy = null, string includeProperties = "")
        {
            List<MembershipScope> items = base.Get(filter, orderBy, includeProperties).ToList();
            if(filter == null)
            {
                foreach(Enum e in Enum.GetValues(typeof(MembershipScopeReserved)))
                {
                    string name = System.Enum.GetName(typeof(MembershipScopeReserved), e);
                    long value = Convert.ToUInt32(e);
                    if (name.ToLower() != "none")
                    {
                        MembershipScope s = new MembershipScope()
                        {
                            Caption = name,
                            BinaryValue = value,
                            ScopeQualifier = MembershipScopeQualifier.Reserved
                        };
                        items.Add(s);
                    }
                }
            }
            return items;
        }

        public List<MembershipScope> GetApplicableScopes(long binaryValue)
        {
            List<MembershipScope> items = new List<MembershipScope>();

            List<MembershipScope> all = Get().OrderBy(x => x.Caption).ToList();
            string[] names = all.Select(x => GenerateLiteralName(x.Caption)).ToArray();

            foreach (MembershipScope s in all)
            {
                string name = GenerateLiteralName(s.Caption);

                Enum sv = (Enum)Enum.ToObject(ScopeEnumType, binaryValue);
                Enum cv = (Enum)Enum.Parse(ScopeEnumType, name);

                string svNames = sv.ToString();
                string cvNames = cv.ToString();

                bool flag = sv.HasFlag(cv);
                bool cmp = ((binaryValue & s.BinaryValue) == binaryValue);

                //36283883716616



                if (sv.HasFlag(cv))
                    items.Add(s);
            }

            return items;
        }

        public string[] GetApplicableCaptions(long binaryValue)
        {
            return GetApplicableScopes(binaryValue).Select(x => x.Caption).ToArray();
        }

        public string GetCaptionLabel(long binaryValue)
        {

            if (binaryValue == 0)
                return "None";
            else
            {
                List<string> caption = new List<string>();

                foreach (string n in Enum.GetNames(typeof(MembershipScopeQualifier)))
                {
                    MembershipScopeQualifier q = (MembershipScopeQualifier)Enum.Parse(typeof(MembershipScopeQualifier), n);

                    if (q != MembershipScopeQualifier.None)
                    {
                        List<MembershipScope> l = GetApplicableScopes(binaryValue).Where(x => x.ScopeQualifier == q).ToList();
                        if (l.Count() > 0)
                        {
                            if ( q== MembershipScopeQualifier.Reserved)
                            
                                caption.Add("(" + string.Join(" + ", l.Select(x => x.Caption)) + ")");
                            else
                                caption.Add("(" + string.Join(" or ", l.Select(x => x.Caption)) + ")");

                        }
                            

                    }
                }

                return string.Join(" and ", caption.ToArray());
            }
        }

        public List<MembershipScope> GetByQualifier(MembershipScopeQualifier q)
        {
            List<MembershipScope> scopes = Get()
                .Where(x => x.ScopeQualifier == q)
                .OrderBy(x => x.Caption).ToList();

            return scopes;
        }

        #endregion

        #region Reserved

        public void ClearReservedValue()
        {
            long initValue = DynamicScopeMinValue();
            List<MembershipScope> scopes = Get(x => x.BinaryValue < initValue).ToList();

            GenericRepository<DirectoryMember> mbrCntxt = new GenericRepository<DirectoryMember>(this.Context);

            foreach (MembershipScope s in scopes)
            {
                long orgValue = s.BinaryValue;

                long newValue = FindFirstAvailableBinaryValue();

                List<DirectoryMember> mm = mbrCntxt.Get(x => (x.MembershipScope & orgValue) == orgValue).ToList();
                foreach (DirectoryMember m in mm)
                {
                    m.MembershipScope -= orgValue;
                    m.MembershipScope += newValue;
                }

                Delete(s);

                s.BinaryValue = newValue;
                Insert(s);
            }
        }

        #endregion

        #region Enum

        public System.Type ScopeEnumType
        {
            get
            {
                if (_membershipScopeType != null)
                    return _membershipScopeType;


                AppDomain currentDomain = AppDomain.CurrentDomain;

                AssemblyName aName = new AssemblyName(this.GetType().Name + "Enum");

                AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

                ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

                EnumBuilder eb = mb.DefineEnum(aName.Name, TypeAttributes.Public, typeof(long));

                List<MembershipScope> items = Get().ToList();
                


                foreach (MembershipScope item in items)
                {

                    string name = GenerateLiteralName(item.Caption);
                    long value = item.BinaryValue;

                    eb.DefineLiteral(name, value);

                }

                


                _membershipScopeType = eb.CreateType();

                string[] names = System.Enum.GetNames(_membershipScopeType);

                

                // other codefieldbuilder fb = eb.defineliteral(name, result.exampleid);var cab = new customattributebuilder(ci, new object[] { result.name });fb.setcustomattribute(cab);

                return _membershipScopeType;
            }
        }

        public static bool HasFlag(long binaryValue, string literalName, System.Type t)
        {
            Enum bv = (Enum)Enum.ToObject(t, binaryValue);
            Enum cv = (Enum)Enum.Parse(t, literalName);
            return cv.HasFlag(bv);
        }

        public bool HasFlag(long memberValue, long challenge, System.Type t)
        {
            Enum mv = (Enum)Enum.ToObject(t, memberValue);
            Enum cv = (Enum)Enum.ToObject(t, challenge);

            return mv.HasFlag(cv);
        }

        public string GetLiteralCaption(long binaryValue)
        {
            Type t = ScopeEnumType;
            Enum bv = (Enum)Enum.ToObject(t, binaryValue);
            return System.Enum.GetName(t, bv);
        }

        public static string GenerateLiteralName(string caption)
        {
            return Regex.Replace(caption, @"[^A-Za-z0-9]+", "");
        }

        #endregion


    }


}
