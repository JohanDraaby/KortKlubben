package game;

public class Card {
    private byte value;
    private String suit;
    private String colour;
    private String fullName;

    public int getValue(){
        return this.value;
    }

    public String getSuit(){
        return this.suit;
    }

    public String getColour(){
        return this.colour;
    }

    public String getFullName(){
        return this.fullName;
    }

    public void setFullName(String newName){
        this.fullName = newName;
    }

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
