package my.vrestate.client.Drawables;

public interface IDrawable {
	public String getKML();
	public void setDrawId(int draw_id);
	public int getDrawId();
	public void setVisible(boolean visible);
	public boolean isVisible();
}
