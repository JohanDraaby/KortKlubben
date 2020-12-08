package game;

import java.util.ArrayList;

public class CardFactory {

    private Card CreateCard(byte value, String suit, String colour){
        return new Card(value, suit, colour);
    }

    public ArrayList[Card] CreateDeck(){
        ArrayList<Card> deck = new ArrayList<>();

        for (byte b = 1; b < 14; b++){
            deck.add(new Card(b, "Hjerter", "Rød"));
            deck.add(new Card(b, "Ruder", "Rød"));
            deck.add(new Card(b, "Klør", "Sort"));
            deck.add(new Card(b, "Spar", "Sort"));
        }
    }
}
