package game;

import java.util.ArrayList;

public class CardFactory {
    /**
     * Bla bla bla bla
     * @param colour Black or red
     * @return Returns a new card
     */
    private Card CreateCard(byte value, String suit, String colour){
        return new Card(value, suit, colour);
    }

    String gayAss () {
        return "";
    }

    public ArrayList<Card>CreateDeck(){
        ArrayList<Card> deck = new ArrayList<>();
        
        for (byte b = 1; b < 14; b++){
            deck.add(new Card(b, "Hjerter", "Rød"));
            deck.add(new Card(b, "Ruder", "Rød"));
            deck.add(new Card(b, "Klør", "Sort"));
            deck.add(new Card(b, "Spar", "Sort"));
        }
        
        deck.add(new Card((byte) 14, "", "Rød"));
        deck.add(new Card((byte) 14, "", "Rød"));
        deck.add(new Card((byte) 14, "", "Sort"));
        deck.add(new Card((byte) 14, "", "Sort"));

        return deck;
    }
}
