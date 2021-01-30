using Jypeli;
using Jypeli.Widgets;
using System.Collections.Generic;

/// @author Linda
/// @version 28.11.19
///
/// <summary>
/// Peli, jossa seikkaillaan kissalla avaruudessa ja yritettän pyydystää hiiriä. 
/// </summary>
public class SpaceCat : PhysicsGame
{
    private EasyHighScore topLista = new EasyHighScore();
    private PhysicsObject pelaaja;
    private List<Label> valikonKohdat;
    private IntMeter pisteLaskuri;
    private DoubleMeter alaspainLaskuri;
    private Timer aikaLaskuri;
    private Image[] kuvat = LoadImages("kissaraja", "hiiru", "avaruustausta", "astroman");


    public override void Begin()
    {
        Valikko();
    }


    /// <summary>
    /// Kun valikossa painetaan "Aloita peli", siirrytään tähän aliohjelmaan, jossa itse peli sitten käynnistyy.
    /// </summary>
    private void Aloita()
    {
        Level.CreateBorders(false);

        BoundingRectangle alue = Level.BoundingRect;

        Level.Background.Image = kuvat[2];

        pelaaja = LuoKissa(this, 0, 0, 50, "pelaaja");

        PhysicsObject hiiri = LuoHiiri(this, alue, 500, "hiiri" );

        PhysicsObject herkku = LuoHerkku(this, alue, 300, "herkku");

        for (int i = 0; i < 5; i++)
        {
            PhysicsObject vihu;
            vihu = LuoAstronautti(this, alue, 300, "vihu", kuvat);
        }

        LuoPistelaskuri();
        LuoAikaLaskuri();
        LuoNappaimet(pelaaja);

        Timer.CreateAndStart(2.0, LisaaHerkkuja);

        AddCollisionHandler(pelaaja, "hiiri", PelaajaTormasi);
        AddCollisionHandler(pelaaja, "vihu", PelaajaTormasiAstronautti);
        AddCollisionHandler(pelaaja, "herkku", PelaajaTormasiHerkku);
    }


    /// <summary>
    /// Luodaan peliin näppäinkomennot.
    /// </summary>
    /// <param name="pelaaja"></param>
    private void LuoNappaimet(PhysicsObject pelaaja)
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä Avustus");
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LyoOliota, "Pelaaja liikkuu ylös", pelaaja, new Vector(0, 200));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LyoOliota, "Pelaaja liikkuu alas", pelaaja, new Vector(0, -200));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LyoOliota, "Pelaaja liikkuu vasemmalle", pelaaja, new Vector(-200, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LyoOliota, "Pelaaja liikkuu oikealle", pelaaja, new Vector(200, 0));
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.R, ButtonState.Pressed, AloitaPeli, "Aloita peli");
    }


    /// <summary>
    /// Aliohjelma, joka luo kissan eli pelaajan peliin.
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="vauhti"></param>
    /// <param name="tunniste"></param>
    /// <returns></returns>
    private PhysicsObject LuoKissa(PhysicsGame peli, double x, double y, double vauhti, string tunniste)
    {
        PhysicsObject kissa = new PhysicsObject(80 * 2, 60 * 2);
        kissa.Shape = Shape.Circle;
        kissa.Color = Color.Orange;
        kissa.X = x;
        kissa.Y = y;
        Vector suunta = RandomGen.NextVector(0, vauhti);
        kissa.Tag = tunniste;
        kissa.Image = kuvat[0];
        kissa.CanRotate = false;
        peli.Add(kissa);
        return kissa;
    }


    /// <summary>
    /// Aliohjelma, joka astronautit eli viholliset peliin.
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="vauhti"></param>
    /// <param name="tunniste"></param>
    /// <returns></returns>
    private PhysicsObject LuoAstronautti(PhysicsGame peli, BoundingRectangle alue, double vauhti, string tunniste, Image[] k)
    {
        PhysicsObject astronautti = new PhysicsObject(50 * 2, 50 * 2);
        astronautti.Shape = Shape.Circle;
        astronautti.Position = RandomGen.NextVector(alue);
        astronautti.Angle = RandomGen.NextAngle();
        Vector suunta = RandomGen.NextVector(0, vauhti);
        astronautti.Hit(suunta);
        astronautti.Color = Color.White;
        astronautti.Tag = tunniste;
        astronautti.Image = k[3];
        peli.Add(astronautti);

        return astronautti;
    }


    /// <summary>
    /// Aliohjelma, joka luo hiiret
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="alue"></param>
    /// <param name="vauhti"></param>
    /// <param name="tunniste"></param>
    /// <returns></returns>
    private PhysicsObject LuoHiiri(PhysicsGame peli, BoundingRectangle alue, double vauhti, string tunniste)
    {
        PhysicsObject hiiri = new PhysicsObject(30 * 2, 30 * 2);
        hiiri.Shape = Shape.Circle;
        // hiiri.Position = Level.GetRandomPosition();
        hiiri.Position = RandomGen.NextVector(alue);
        hiiri.Angle = RandomGen.NextAngle();
        Vector suunta = RandomGen.NextVector(0, vauhti);
        hiiri.Hit(suunta);
        hiiri.Tag = tunniste;
        hiiri.Image = kuvat[1];
        hiiri.Color = Color.Gray;
        peli.Add(hiiri);

        return hiiri;
    }


    /// <summary>
    /// Aliohjelma, joka luo herkut.
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="alue"></param>
    /// <param name="vauhti"></param>
    /// <param name="tunniste"></param>
    /// <returns></returns>
    private PhysicsObject LuoHerkku(PhysicsGame peli, BoundingRectangle alue, double vauhti, string tunniste)
    {
        PhysicsObject herkku = new PhysicsObject(15 * 2, 15 * 2);
        herkku.Shape = Shape.Circle;
        herkku.Position = RandomGen.NextVector(alue);
        herkku.Angle = RandomGen.NextAngle();
        Vector suunta = RandomGen.NextVector(0, vauhti);
        herkku.Hit(suunta);
        herkku.Tag = tunniste;
        herkku.Image = LoadImage("alien2");
        herkku.LifetimeLeft = System.TimeSpan.FromSeconds(3.0);
        peli.Add(herkku);

        return herkku;
    }


    /// <summary>
    /// Aliohjelma, joka saa "kissan" liikkeelle.
    /// </summary>
    /// <param name="pallo"></param>
    /// <param name="suunta"></param>
    private static void LyoOliota(PhysicsObject pallo, Vector suunta)
    {
        pallo.Hit(suunta);
    }


    /// <summary>
    /// Aliohjelma, jossa määrätään mitä tapahtuu, kun "kissa" osuu "hiireen".
    /// </summary>
    /// <param name="kissa"></param>
    /// <param name="hiiri"></param>
    private void PelaajaTormasi(PhysicsObject kissa, PhysicsObject hiiri)
    {
        BoundingRectangle alue = Level.BoundingRect;

        hiiri.Destroy();
        pisteLaskuri.Value += 1;
        alaspainLaskuri.Value += 2;
        LuoHiiri(this, alue, 500, "hiiri");
    }


    /// <summary>
    /// Aliohjelma, jossa määrätään mitä tapahtuu, "kissa" törmää "astronauttiin"
    /// </summary>
    /// <param name="kissa"></param>
    /// <param name="astronautti"></param>
    private void PelaajaTormasiAstronautti(PhysicsObject kissa, PhysicsObject astronautti)
    {
        astronautti.Destroy();
        alaspainLaskuri.Value -= 5.0;
        LuoAstronautti(this, Level.BoundingRect, 300, "vihu", kuvat);
    }


    /// <summary>
    /// Aliohjelma, jossa määrätään mitä tapahtuu, kun "kissa" törmää "herkkuun". 
    /// </summary>
    /// <param name="kissa"></param>
    /// <param name="herkku"></param>
    private void PelaajaTormasiHerkku(PhysicsObject kissa, PhysicsObject herkku)
    {
        herkku.Destroy();
        alaspainLaskuri.Value += 5.0;
    }


    /// <summary>
    /// Aliohjelma, joka hoitaa pisteiden laskun.
    /// </summary>
    private void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Yellow;
        pisteNaytto.BorderColor = Color.Yellow;
        pisteNaytto.Title = "Pisteet";

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// Aliohjelma, joka luo aikalaskurin.
    /// </summary>
    private void LuoAikaLaskuri()
    {
        alaspainLaskuri = new DoubleMeter(30);

        aikaLaskuri = new Timer();
        aikaLaskuri.Interval = 0.1;
        aikaLaskuri.Timeout += LaskeAlaspain;
        aikaLaskuri.Start();

        Label aikaNaytto = new Label();
        aikaNaytto.DecimalPlaces = 0;
        aikaNaytto.BindTo(alaspainLaskuri);
        aikaNaytto.X = Screen.Left + 100;
        aikaNaytto.Y = Screen.Top - 75;
        aikaNaytto.TextColor = Color.Yellow;
        Add(aikaNaytto);
    }


    /// <summary>
    /// Aliohjelma, jonka avulla aikalaskuri laskee alaspäin.
    /// </summary>
    private void LaskeAlaspain()
    {
        alaspainLaskuri.Value -= 0.1;

        if (alaspainLaskuri.Value <= 0)
        {
            MessageDisplay.Add("Aika loppui...");
            aikaLaskuri.Stop();

            AikaLoppui();
        }
    }


    /// <summary>
    /// Aliohjelmassa määrätään mitä tapahtuu, kun aika loppuu pelissä. 
    /// </summary>
    private void AikaLoppui()
    {
        ClearAll();
        Level.Background.Image = kuvat[2];
        Valikko();
        topLista.EnterAndShow(pisteLaskuri.Value);
    }


    /// <summary>
    /// Aliohjelma, joka lisää herkkuja pelikentälle tietyn ajan välein.
    /// </summary>
    private void LisaaHerkkuja()
    {
        LuoHerkku(this, Level.BoundingRect, 50, "herkku");
    }


    /// <summary>
    /// Luodaan peliin alkuvalikko.
    /// </summary>
    private void Valikko()
    {
        ClearAll();
        Level.Background.Image = kuvat[2];
        LuoOtsikko();

        valikonKohdat = new List<Label>(); // Alustetaan lista, johon valikon kohdat tulevat

        Label kohta1 = new Label("Aloita uusi peli");  // Luodaan uusi Label-olio, joka toimii uuden pelin aloituskohtana
        kohta1.Position = new Vector(0, -40);  // Asetetaan valikon ensimmäinen kohta hieman kentän keskikohdan yläpuolelle
        kohta1.Font = Font.DefaultLarge;
        valikonKohdat.Add(kohta1);  // Lisätään luotu valikon kohta listaan jossa kohtia säilytetään

        Label kohta2 = new Label("Parhaat pisteet");
        kohta2.Position = new Vector(0, -120);
        kohta2.Font = Font.DefaultLarge;
        valikonKohdat.Add(kohta2);

        Label kohta3 = new Label("Lopeta peli");
        kohta3.Position = new Vector(0, -200);
        kohta3.Font = Font.DefaultLarge;
        valikonKohdat.Add(kohta3);

        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }

        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, ParhaatPisteet, null);
        Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, Exit, null);
        Mouse.ListenMovement(0.0, ValikossaLiikkuminen, null);

    }


    /// <summary>
    /// Lisätään peliin widget, jonka avulla saadaan pelin otsikko näkymään valikon kanssa. 
    /// </summary>
    private void LuoOtsikko()
    {
        Widget ruutu = new Widget(700.0, 500.0);
        ruutu.X = 0;
        ruutu.Y = 150;
        ruutu.Image = LoadImage("otsikko");
        Add(ruutu);
    }


    /// <summary>
    /// Siirrytään tähän aliohjelmaan, kun valikosta painetaan "Aloita peli".
    /// </summary>
    private void AloitaPeli()
    {
        ClearAll();
        Aloita();
    }


    /// <summary>
    /// Siirrytään tähän aliohjelmaan, kun valikosta painetaan "Parhaat pisteet".
    /// </summary>
    private void ParhaatPisteet()
    {
        topLista.Show();
    }


    /// <summary>
    /// Mahdollistetaan valikossa hiirellä liikkuminen.
    /// </summary>
    /// <param name="hiirenTila"></param>
    private void ValikossaLiikkuminen(AnalogState hiirenTila)
    {
        foreach (Label kohta in valikonKohdat)
        {
            if (Mouse.IsCursorOn(kohta))
            {
                Color vari = Color.Yellow;
                kohta.TextColor = Color.Darker(vari, 5);
            }
            else
            {
                kohta.TextColor = Color.White;
            }

        }
    }


}
