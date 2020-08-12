using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Z3;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace z3test
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        [JsonProperty("class")]
        public int wowClass { get; set; }
        public int subclass { get; set; }
        public int quality { get; set; }
        public int slot { get; set; }
        public int reqLevel { get; set; }
        public int durability { get; set; }
        public double meleeDps { get; set; }
        public double meleeSpeed { get; set; }
        public double meleeMinDmg { get; set; }
        public double meleeMaxDmg { get; set; }
        public int phase { get; set; }
        public int armor { get; set; }
        public bool bop { get; set; }
        public int strength { get; set; }
        public int stamina { get; set; }
        // public int validSuffixIds { get; set; }
        public string boss { get; set; }
        public int agility { get; set; }
        public bool unique { get; set; }
        public int spirit { get; set; }
        public int intellect { get; set; }
        public int natureResistance { get; set; }
        public int hp5 { get; set; }
        public int spellDamage { get; set; }
        public int meleeCrit { get; set; }
        public int rangedCrit { get; set; }
        public int attackPower { get; set; }
        public int defense { get; set; }
        public int blockValue { get; set; }
        public int shadowResistance { get; set; }
        public int frostDamage { get; set; }
        public int frostResistance { get; set; }
        public int fireDamage { get; set; }
        public int fireResistance { get; set; }
        public int shadowDamage { get; set; }
        public int arcaneResistance { get; set; }
        public int natureDamage { get; set; }
        public string flavor { get; set; }
        public int blockChance { get; set; }
        public int rangedDps { get; set; }
        public int rangedSpeed { get; set; }
        public int rangedMinDmg { get; set; }
        public int rangedMaxDmg { get; set; }
        public int spellHealing { get; set; }
        public int[] allowableClasses { get; set; }
        public int dodge { get; set; }
        public int spellCrit { get; set; }
        public int parry { get; set; }
        public int arcaneDamage { get; set; }
        public int mp5 { get; set; }
        public int rangedAttackPower { get; set; }
        public int meleeHit { get; set; }
        public int rangedHit { get; set; }
        public int spellHit { get; set; }
        public int pvpRank { get; set; }
        public int spellPenetration { get; set; }
        public int holyDamage { get; set; }
        public int targetMask { get; set; }
        public int feralAttackPower { get; set; }
    }

    public class ResultItem
    {
        public string name { get; set; }
        public int itemid { get; set; }
        public double equivSP { get; set; }
    }
    public class Result
    {
        public List<ResultItem> items { get; set; }
        public double equivSp { get; set; }
        public double crit { get; set; }
        public double hit { get; set; }
        public double rawSp { get; set; }
    }

    public class Program
    {
        public static Result Calculate(double oneCritEquals, bool staffsTrueDaggerFalse, HashSet<int> excludeItems, bool listIsExcludeMode, HashSet<int> requireUse)
        {
            // FIXME: in the data, is spellCrit a double or int?
            // https://github.com/ultrabis/db
            List<Item> allItems = new List<Item>();
            JArray database = (JArray)JsonConvert.DeserializeObject(File.ReadAllText("item-modular.json"));

            // We want to find which slots the requireUse items use, then only add that one option for those slots.
            HashSet<int> noOptionSlots = new HashSet<int>();

            foreach (JObject item in database)
            {
                Item i = item.ToObject<Item>();

                // remap one type of off hands to the other type.
                if (i.slot == 21)
                {
                    i.slot = 13;
                }

                if (requireUse.Contains(i.id))
                {
                    noOptionSlots.Add(i.slot);
                    if (i.slot == 17)
                    {
                        // if its a staff, exclude mainhands/offhands
                        noOptionSlots.Add(13);
                        noOptionSlots.Add(23);
                    }
                    if (i.slot == 13)
                    {
                        // if its a mainhand, exclude staffs
                        noOptionSlots.Add(17);
                    }
                    if (i.slot == 23)
                    {
                        // if its an offhand, exclude staffs
                        noOptionSlots.Add(17);
                    }
                }

            }

            foreach (JObject item in database)
            {
                Item i = item.ToObject<Item>();

                // remap one type of off hands to the other type.
                if (i.slot == 21)
                {
                    i.slot = 13;
                }

                // If there is only 1 choice for a slot, check it is this item.
                if (noOptionSlots.Contains(i.slot))
                {
                    if (!requireUse.Contains(i.id))
                    {
                        continue;
                    }
                }

                if (listIsExcludeMode)
                {
                    // exclude mode
                    if (excludeItems.Contains(i.id))
                    {
                        continue;
                    }
                } else
                {
                    // include only
                    if (!excludeItems.Contains(i.id))
                    {
                        continue;
                    }
                }
                

                //if (i.reqLevel != 60)
                //{
                //    continue;
                //}

                // No point checking shirt or tabard or containers or BUGGED SLOT
                //if (i.slot == 4 || i.slot == 19)
                //{
                //    continue;
                //}

                //if (i.slot == 28 || i.slot == 18 || i.slot == 4 || i.slot == 25)
                //{
                //    continue; // skip, can't use librams
                //}

                if (i.name == "Rockfury Bracers")
                {
                    // No idea why the data is wrong.
                    i.phase = 5;
                }

                // FIXME: work out how to solve weapon choice
                bool useStaffs = staffsTrueDaggerFalse;
                if (useStaffs)
                {
                    if (i.slot == 13 || i.slot == 23)
                    {
                        // skip 1h, mainhand and offhands. only consider staffs for now.
                        continue;
                    }
                }
                else
                {
                    if (i.slot == 17)
                    {
                        // skip staff
                        continue;
                    }
                }

                if (i.phase == 6)
                {
                    // naxx not out yet
                    continue;
                }

                // exclude leather, mail, plate.
                if (i.wowClass == 4)
                {
                    if (i.subclass == 2 || i.subclass == 3 || i.subclass == 4 || i.subclass == 6 || i.subclass == 7 || i.subclass == 8 || i.subclass == 9 || i.subclass == 10 || i.subclass == 11)
                    {
                        continue;
                    }
                }
                else if (i.wowClass == 2)
                {// weapons
                    if (i.subclass == 4 || i.subclass == 5 || i.subclass == 8 || i.subclass == 2)
                    {
                        continue;
                    }
                }


                // skip absoltely useless items
                // add MQG as otherwise it will be missing
                if (i.spellDamage == 0 && i.spellCrit == 0 && i.spellHit == 0 && i.intellect == 0 && i.fireDamage == 0 && i.id != 19339)
                {
                    continue;
                }

                if (i.allowableClasses != null && i.allowableClasses.Count() > 0 && !i.allowableClasses.Contains(8))
                {
                    // 8 = mage
                    continue;
                }

                allItems.Add(i);

            }

            // If no staff or no mainhand/offhadn is included and this is a run for that weapon type, then throw nosat. If we continue it will error saying slot-17 not defined.
            if (staffsTrueDaggerFalse && !allItems.Where((x) => x.slot == 17).Any())
            {
                return new Result()
                {
                    items = new List<ResultItem>(),
                    equivSp = 0
                };
            }
            if (!staffsTrueDaggerFalse && 
                (  !allItems.Where((x) => x.slot == 13).Any()
                || !allItems.Where((x) => x.slot == 23).Any()
                ))
            {
                return new Result()
                {
                    items = new List<ResultItem>(),
                    equivSp = 0
                };
            }


            using (Context ctx = new Context(new Dictionary<string, string>() { { "model", "true" } }))
            {
                Goal g = ctx.MkGoal(true);

                DatatypeSort itemType;
                {
                    // (declare-datatypes () ((Item(mk-item (id Int) (slot Slot) (spellDamage Int) (spellCrit Int) (spellHit Int)))))
                    Sort intSort = ctx.MkIntSort();
                    Sort[] sorts = { intSort, intSort, intSort, intSort, intSort, intSort };
                    string[] fields = { "id", "slot", "int", "spellDamage", "spellCrit", "spellHit" };
                    Constructor c = ctx.MkConstructor("mk-item", "mk-item", fields, sorts);
                    Constructor[] cs = { c };
                    itemType = ctx.MkDatatypeSort("Item", cs);
                }

                double hitCoeff = 15.0;
                double critCoeff = oneCritEquals;
                
                // One mage, WB
                //double hitCoeff = 17.5;
                //double critCoeff = 13.3;

                // 6 mages, WB

                //double hitCoeff = 15.1;
                //double critCoeff = 10.0;

                Dictionary<int, List<Expr>> itemsInSlots = new Dictionary<int, List<Expr>>();

                foreach (Item i in allItems)
                {

                    // (delcare-const item-id Item)
                    // FIXME: can I use item-{i.name}? or will spaces mess it up.
                    Symbol s = ctx.MkSymbol($"item-{i.id}");
                    var c = ctx.MkConst(s, itemType);

                    // (assert (= item-id (mk-item ...)))
                    IntExpr iid = ctx.MkInt(i.id);
                    IntExpr islot = ctx.MkInt(i.slot);
                    IntExpr iint = ctx.MkInt(i.intellect);
                    // FIXME: instead of doing the calculation in the maximize function, im just doing it here. should of just used a for loop becaues declare-func doesn't work... :(
                    // equiv. damage
                    // 59.5 int = 1 crit.
                    double intcrit = ((double)i.intellect) / 59.5;
                    intcrit += i.spellCrit;
                    double edmg = (double)i.spellDamage * 1000;
                    edmg += (double)i.fireDamage * 1000;
                    edmg += (critCoeff * 1000) * intcrit;
                    edmg += (hitCoeff * 1000) * i.spellHit;

                    IntExpr ispellDamage = ctx.MkInt((int)Math.Round(edmg));
                    IntExpr ispellCrit = ctx.MkInt(i.spellCrit);
                    IntExpr ispellHit = ctx.MkInt(i.spellHit);

                    Expr[] exprs = { iid, islot, iint, ispellDamage, ispellCrit, ispellHit };
                    Expr itemExpr = ctx.MkApp(itemType.Constructors[0], exprs);
                    g.Assert(ctx.MkEq(c, itemExpr));
                    if (!itemsInSlots.ContainsKey(i.slot))
                    {
                        itemsInSlots[i.slot] = new List<Expr>();
                    }
                    var existing = itemsInSlots[i.slot];
                    existing.Add(c);
                    itemsInSlots[i.slot] = existing;
                }

                {
                    List<Symbol> slotSymbols = new List<Symbol>();
                    List<Expr> slotDecls = new List<Expr>();
                    foreach (int slot in itemsInSlots.Keys)
                    {
                        // rings
                        if (slot == 11)
                        {

                            AddSLot(ctx, g, itemType, itemsInSlots, slotSymbols, slotDecls, slot, "11-1");
                            AddSLot(ctx, g, itemType, itemsInSlots, slotSymbols, slotDecls, slot, "11-2");
                        }
                        else if (slot == 12)
                        {
                            // trinkets
                            AddSLot(ctx, g, itemType, itemsInSlots, slotSymbols, slotDecls, slot, "12-1");
                            // For mages: disable the 2nd trinket slot, use MQG.

                            AddSLot(ctx, g, itemType, itemsInSlots, slotSymbols, slotDecls, slot, "12-2");
                        }
                        else
                        {
                            AddSLot(ctx, g, itemType, itemsInSlots, slotSymbols, slotDecls, slot, slot.ToString());
                        }
                    }

                    // define it anyway so the code parses, even though it won't have an item assignment.
                    if (staffsTrueDaggerFalse)
                    {
                        {
                            Symbol s = ctx.MkSymbol($"slot-13");
                            var c = ctx.MkConst(s, itemType);
                            slotSymbols.Add(s);
                            slotDecls.Add(c);
                        }
                        {
                            Symbol s = ctx.MkSymbol($"slot-23");
                            var c = ctx.MkConst(s, itemType);
                            slotSymbols.Add(s);
                            slotDecls.Add(c);
                        }

                    } else
                    {
                        Symbol s = ctx.MkSymbol($"slot-17");
                        var c = ctx.MkConst(s, itemType);
                        slotSymbols.Add(s);
                        slotDecls.Add(c);
                    }



                    Symbol usestaffs = ctx.MkSymbol("usestaffs");
                    var usestaffsConst = ctx.MkConst(usestaffs, ctx.IntSort);
                    g.Assert(ctx.MkEq(usestaffsConst, ctx.MkInt(staffsTrueDaggerFalse ? 1 : 0)));

                    Symbol spellcritValue = ctx.MkSymbol("spellcrit-value");
                    var spellcritValueConst = ctx.MkConst(spellcritValue, ctx.IntSort);
                    g.Assert(ctx.MkEq(spellcritValueConst, ctx.MkInt((int)Math.Round(critCoeff * 1000))));

                    var passSymbols = slotSymbols.ToArray();
                    passSymbols = passSymbols.Append(usestaffs).ToArray();
                    passSymbols = passSymbols.Append(spellcritValue).ToArray();

                    var passsDecls = slotDecls.Select((x) => x.FuncDecl).ToArray();
                    passsDecls = passsDecls.Append(usestaffsConst.FuncDecl).ToArray();
                    passsDecls = passsDecls.Append(spellcritValueConst.FuncDecl).ToArray();

                    //Symbol[] declNames = { c };
                    //FuncDecl[] decls = new FuncDecl[] { c.FuncDecl };
                    Symbol[] sortNames = new Symbol[] { itemType.Name };
                    Sort[] sorts = new Sort[] { itemType };
                    // Add in any additional constraints.
                    string fileContent = File.ReadAllText("itemscore.smt2");
                    BoolExpr[] fmls = ctx.ParseSMTLIB2String(fileContent, sortNames, sorts, passSymbols, passsDecls);
                    BoolExpr and = ctx.MkAnd(fmls);
                    g.Assert(and);

                    // Console.WriteLine("Goal: " + g);

                    Optimize solver = ctx.MkOptimize();
                    //var ps = ctx.MkParams();
                    //ps.Add("parallel.enable", true);
                    //ps.Add("priority", ctx.MkSymbol("pareto"));
                    //solver.Parameters = ps;
                    // Solver solver = ctx.MkSolver();

                    foreach (BoolExpr a in g.Formulas)
                        solver.Assert(a);

                    // (maximize (score slot-head))
                    // (maximize (id slot-head))
                    // FIXME: instead of maximizing each slot, I should maximize a variable inside itemscore.smt2 that is the sum of each slots spelldamage (and only picks either staff or mainhand/offhand).
                    //foreach (var slot in slotDecls)
                    //{
                    //    var handle = solver.MkMaximize(ctx.MkApp(itemType.Accessors[0][3], slot));
                    //}

                    var handle = solver.MkMaximize(ctx.MkConst(ctx.MkSymbol("total-spev"), ctx.IntSort));

                    if (solver.Check() != Status.SATISFIABLE)
                        return new Result()
                        {
                            items = new List<ResultItem>(),
                            equivSp = 0
                        };

                    double total = 0;

                    double itemSP = 0;
                    double itemCrit = 0;
                    double itemHit = 0;

                    int zanzilSet = 0;
                    int pvpSet = 0;
                    int bvSet = 0;

                    int totalHit = 0;
                    int totalCrit = 0;
                    Console.WriteLine($"Hit coeff: {hitCoeff}");
                    Console.WriteLine($"Crit coeff: {critCoeff}");
                    var r = new Result();
                    r.items = new List<ResultItem>();
                    foreach (var slot in slotSymbols.OrderBy((x) => int.Parse(x.ToString().Split('-')[1])))
                    {
                        var itemExprs = solver.Model.Consts.Where((x) => x.Key.Name == slot);
                        if (itemExprs.Count() == 0)
                        {
                            continue;
                        }
                        var itemExpr = itemExprs.First();
                        var idexpr = itemExpr.Value.Args[0];
                        var spevexpr = itemExpr.Value.Args[3];
                        var dbitem = allItems.Find((x) => x.id == ((IntNum)idexpr).Int);
                        double esp = ((IntNum)spevexpr).Int;
                        //Console.WriteLine($"{esp / 1000.0}\t");
                        //Console.WriteLine($"{dbitem.name}");
                        total += ((IntNum)spevexpr).Int;
                        totalHit += dbitem.spellHit;
                        totalCrit += dbitem.spellCrit;

                        itemSP += dbitem.spellDamage + dbitem.fireDamage;
                        itemCrit += dbitem.spellCrit + (((double)dbitem.intellect) / 59.5);
                        itemHit += dbitem.spellHit;

                        if (dbitem.id == 19905 || dbitem.id == 19893)
                        {
                            zanzilSet += 1;
                        }

                        if (dbitem.id == 23319 || dbitem.id == 23291)
                        {
                            pvpSet += 1;
                        }

                        if (dbitem.id == 19682 || dbitem.id == 19683 || dbitem.id == 19684)
                        {
                            bvSet += 1;
                        }

                        r.items.Add(new ResultItem() {
                            name = dbitem.name,
                            itemid = dbitem.id,
                            equivSP = esp / 1000.0
                        });
                    }

                    //Console.WriteLine($"sp: {itemSP}");
                    //Console.WriteLine($"crit: {itemCrit}");
                    //Console.WriteLine($"hit: {itemHit}");
                    //Console.WriteLine($"total equiv. SP: {((double)((IntNum)handle.Upper).Int) / 1000.0} | Hit: {totalHit} | Crit: {totalCrit}");

                    r.crit = itemCrit;
                    r.hit = itemHit;
                    r.equivSp = ((double)((IntNum)handle.Upper).Int) / 1000.0;
                    r.rawSp = itemSP;

                    // have to add back in set bonuses here for display purposes...
                    if (zanzilSet == 2)
                    {
                        r.hit += 1;
                        r.rawSp += 2.0;
                    }

                    if (pvpSet == 2)
                    {
                        r.rawSp += 23;
                    }

                    if (bvSet == 3)
                    {
                        r.crit += 2.0;
                    }
                    return r;
                }


            }

        }

        private static void AddSLot(Context ctx, Goal g, DatatypeSort itemType, Dictionary<int, List<Expr>> itemsInSlots, List<Symbol> slotSymbols, List<Expr> slotDecls, int slot, string slotName)
        {
            Symbol s = ctx.MkSymbol($"slot-{slotName}");
            var c = ctx.MkConst(s, itemType);

            List<BoolExpr> allItemsEqual = new List<BoolExpr>();
            foreach (Expr isym in itemsInSlots[slot])
            {
                allItemsEqual.Add(ctx.MkEq(c, isym));
            }
            slotSymbols.Add(s);
            slotDecls.Add(c);
            g.Assert(ctx.MkOr(allItemsEqual));
        }

    }
}
