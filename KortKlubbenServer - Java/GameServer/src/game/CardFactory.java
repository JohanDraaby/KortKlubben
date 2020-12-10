package game;

import java.util.ArrayList;
/**
 * This class is responsible for instantiating new cards
 */
public class CardFactory {
    /**
     * @param value
     * @param suit
     * @param colour
     * @return Returns New {@link Card} Object
     */
    private Card CreateCard(byte value, String suit, String colour){
        return new Card(value, suit, colour);
    }


    /**
     *
     * @return Returns ArrayList<{@link Card}> Containing 52 Cards
     */
    public ArrayList<Card>CreateDeck(){
        ArrayList<Card> deck = new ArrayList();

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
