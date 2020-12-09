package game;

/**
 * Represents a standard playing card
 */
public class Card {
    private byte value;
    private String suit;
    private String colour;
    private String fullName;

    /**
     *
     * @return Value of {@link Card}
     */
    public int getValue(){
        return this.value;
    }

    /**
     *
     * @return Suit of {@link Card}
     */
    public String getSuit(){
        return this.suit;
    }

    /**
     *
     * @return Colour of {@link Card}
     */
    public String getColour(){
        return this.colour;
    }

    /**
     *
     * @return FullName of {@link Card}
     */
    public String getFullName(){
        return this.fullName;
    }

    /**
     * Sets a new name to the {@link Card}
     * @param newName
     */
    public void setFullName(String newName){
        this.fullName = newName;
    }


    /**
     * Used To Initiate An Object Of Type {@link Card}
     * @param value
     * @param suit
     * @param colour
     */
    public Card(byte value, String suit, String colour) {
        this.value = value;
        this.suit = suit;
        this.fullName = suit + " ";
        this.colour = colour;

        switch (value){
            case 1:
                this.fullName += "Es";
                break;
            case 11:
                this.fullName += "Knægt";
                break;
            case 12:
                this.fullName += "Dame";
                break;
            case 13:
                this.fullName += "Konge";
                break;
            case 14:
                this.fullName = "Joker";
                break;
            default:
                this.fullName += value;
                break;


        }
    }
}
